using System.Net;
using System.Net.Mail;
using Congratulator.Api.Abstractions;
using Congratulator.Api.Options;
using Microsoft.Extensions.Options;

namespace Congratulator.Api.Notifications;

/// <summary>
/// Отправка по e-mail через SMTP. Используется, когда в конфигурации задан Smtp:Host
/// (см. логику выбора реализации в Program.cs). Каждому получателю письмо уходит
/// отдельно через Bcc, чтобы получатели не видели чужие адреса.
/// </summary>
public class SmtpMessageSender : IMessageSender
{
    private readonly IOptionsMonitor<NotificationSettings> _settings;
    private readonly ILogger<SmtpMessageSender> _logger;

    public SmtpMessageSender(IOptionsMonitor<NotificationSettings> settings, ILogger<SmtpMessageSender> logger)
    {
        _settings = settings;
        _logger = logger;
    }

    public async Task SendAsync(IReadOnlyCollection<string> recipients, string subject, string body, CancellationToken ct = default)
    {
        if (recipients.Count == 0)
        {
            return;
        }

        var smtp = _settings.CurrentValue.Smtp;

        using var client = new SmtpClient(smtp.Host, smtp.Port)
        {
            EnableSsl = smtp.EnableSsl,
            Credentials = string.IsNullOrEmpty(smtp.User)
                ? null
                : new NetworkCredential(smtp.User, smtp.Password),
        };

        using var message = new MailMessage
        {
            From = new MailAddress(smtp.FromAddress, smtp.FromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = false,
        };

        foreach (var recipient in recipients)
        {
            message.Bcc.Add(recipient);
        }

        // На случай если Bcc не поддерживается провайдером без основного To — указываем себя отправителем.
        message.To.Add(message.From);

        _logger.LogInformation("Отправка дайджеста ДР по SMTP {Host}:{Port} получателям: {Recipients}",
            smtp.Host, smtp.Port, string.Join(", ", recipients));

        await client.SendMailAsync(message, ct);
    }
}
