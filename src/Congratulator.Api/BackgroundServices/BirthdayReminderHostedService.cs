using Congratulator.Api.Abstractions;
using Congratulator.Api.Options;
using Microsoft.Extensions.Options;

namespace Congratulator.Api.BackgroundServices;

/// <summary>
/// Реализует "автоматическую рассылку сообщений ... в соответствии со сконфигурированным
/// расписанием" из уровня 5 задания. Раз в минуту проверяет: включена ли рассылка,
/// наступило ли сконфигурированное время (Notifications:DailyRunTime) и не запускалась
/// ли она уже сегодня — и если да, выполняет один прогон через IBirthdayNotificationService.
///
/// DbContext не thread-safe и живёт в scoped-времени жизни, поэтому на каждый прогон
/// создаётся отдельный IServiceScope (так положено работать с scoped-сервисами внутри
/// singleton BackgroundService).
/// </summary>
public class BirthdayReminderHostedService : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromMinutes(1);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptionsMonitor<NotificationSettings> _settings;
    private readonly ILogger<BirthdayReminderHostedService> _logger;
    private readonly TimeProvider _timeProvider;

    private DateOnly? _lastRunDate;

    public BirthdayReminderHostedService(
        IServiceScopeFactory scopeFactory,
        IOptionsMonitor<NotificationSettings> settings,
        ILogger<BirthdayReminderHostedService> logger,
        TimeProvider timeProvider)
    {
        _scopeFactory = scopeFactory;
        _settings = settings;
        _logger = logger;
        _timeProvider = timeProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Фоновый сервис рассылки ДР запущен (проверка раз в {Interval}).", PollInterval);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await TickAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                // Сбой одного прогона (например, временная недоступность SMTP) не должен
                // останавливать весь хост — логируем и пробуем снова на следующем тике.
                _logger.LogError(ex, "Ошибка при проверке/отправке рассылки ДР.");
            }

            try
            {
                await Task.Delay(PollInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // нормальное завершение при остановке приложения
            }
        }
    }

    private async Task TickAsync(CancellationToken ct)
    {
        var settings = _settings.CurrentValue;
        if (!settings.Enabled)
        {
            return;
        }

        var now = _timeProvider.GetLocalNow();
        var today = DateOnly.FromDateTime(now.DateTime);

        if (_lastRunDate == today)
        {
            return; // уже отправляли сегодня
        }

        if (!TimeSpan.TryParse(settings.DailyRunTime, out var runTime))
        {
            _logger.LogWarning("Notifications:DailyRunTime = '{Value}' не распознано как HH:mm, рассылка не запланирована.", settings.DailyRunTime);
            return;
        }

        if (now.TimeOfDay < runTime)
        {
            return; // сконфигурированное время ещё не наступило
        }

        using var scope = _scopeFactory.CreateScope();
        var notificationService = scope.ServiceProvider.GetRequiredService<IBirthdayNotificationService>();
        var sentCount = await notificationService.SendDueNotificationsAsync(ct);

        _lastRunDate = today;
        _logger.LogInformation("Плановая рассылка ДР выполнена: уведомлены о {Count} имениннике(ах).", sentCount);
    }
}
