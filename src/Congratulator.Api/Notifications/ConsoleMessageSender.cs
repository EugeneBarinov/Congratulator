using Congratulator.Api.Abstractions;

namespace Congratulator.Api.Notifications;

/// <summary>
/// Реализация-заглушка: вместо реальной отправки пишет письмо в лог.
/// Регистрируется автоматически, если в конфигурации не задан Smtp:Host
/// (см. Program.cs) — это позволяет запустить и продемонстрировать проект
/// без настройки реального почтового сервера.
/// </summary>
public class ConsoleMessageSender : IMessageSender
{
    private readonly ILogger<ConsoleMessageSender> _logger;

    public ConsoleMessageSender(ILogger<ConsoleMessageSender> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(IReadOnlyCollection<string> recipients, string subject, string body, CancellationToken ct = default)
    {
        _logger.LogWarning(
            "SMTP не настроен (Notifications:Smtp:Host пуст) — письмо НЕ отправлено реально, выводится в лог.\n" +
            "Получатели: {Recipients}\nТема: {Subject}\n{Body}",
            string.Join(", ", recipients), subject, body);

        return Task.CompletedTask;
    }
}
