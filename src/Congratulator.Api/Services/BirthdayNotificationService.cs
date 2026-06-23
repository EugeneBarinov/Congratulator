using System.Globalization;
using System.Text;
using Congratulator.Api.Abstractions;
using Congratulator.Api.Dtos;
using Congratulator.Api.Options;
using Microsoft.Extensions.Options;

namespace Congratulator.Api.Services;

public class BirthdayNotificationService : IBirthdayNotificationService
{
    private readonly IPersonService _personService;
    private readonly IMessageSender _messageSender;
    private readonly IOptionsMonitor<NotificationSettings> _settings;
    private readonly ILogger<BirthdayNotificationService> _logger;

    public BirthdayNotificationService(
        IPersonService personService,
        IMessageSender messageSender,
        IOptionsMonitor<NotificationSettings> settings,
        ILogger<BirthdayNotificationService> logger)
    {
        _personService = personService;
        _messageSender = messageSender;
        _settings = settings;
        _logger = logger;
    }

    public async Task<int> SendDueNotificationsAsync(CancellationToken ct = default)
    {
        var settings = _settings.CurrentValue;

        if (settings.Recipients.Count == 0)
        {
            _logger.LogInformation("Рассылка пропущена: список получателей пуст (Notifications:Recipients).");
            return 0;
        }

        var all = await _personService.GetAllAsync(ct);
        var relevant = all
            .Where(p => p.IsToday || (p.DaysUntilNextBirthday > 0 && p.DaysUntilNextBirthday <= settings.UpcomingDaysThreshold))
            .OrderBy(p => p.DaysUntilNextBirthday)
            .ToList();

        if (relevant.Count == 0)
        {
            _logger.LogInformation("Рассылка пропущена: сегодня и в ближайшие {Days} дн. дней рождения нет.", settings.UpcomingDaysThreshold);
            return 0;
        }

        var (subject, body) = ComposeDigest(relevant);
        await _messageSender.SendAsync(settings.Recipients, subject, body, ct);

        return relevant.Count;
    }

    private static (string Subject, string Body) ComposeDigest(IReadOnlyList<PersonDto> people)
    {
        var todayCount = people.Count(p => p.IsToday);
        var subject = todayCount > 0
            ? $"Поздравлятор: сегодня {todayCount} день(дней) рождения!"
            : "Поздравлятор: скоро день(дни) рождения";

        var sb = new StringBuilder();
        sb.AppendLine("Доброе утро! Вот сводка по дням рождения:");
        sb.AppendLine();

        foreach (var person in people)
        {
            var when = person.IsToday
                ? "СЕГОДНЯ"
                : $"через {person.DaysUntilNextBirthday} дн. ({person.NextOccurrence.ToString("d MMMM", CultureInfo.GetCultureInfo("ru-RU"))})";

            sb.AppendLine($"• {person.LastName} {person.FirstName} — {when}, исполняется {person.Age} лет"
                + (string.IsNullOrWhiteSpace(person.Relation) ? string.Empty : $" [{person.Relation}]"));
        }

        sb.AppendLine();
        sb.AppendLine("— Это автоматическое сообщение от приложения «Поздравлятор».");

        return (subject, sb.ToString());
    }
}
