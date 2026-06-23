namespace Congratulator.Api.Abstractions;

/// <summary>
/// Абстракция транспорта рассылки. В задании сказано: "выбор транспорта (email,
/// мессенджеры и т.п.) остаётся за разработчиком" — в проекте это e-mail (SmtpMessageSender),
/// но сервис уведомлений зависит только от этого интерфейса (Dependency Inversion),
/// поэтому добавить, например, отправку в Telegram — вопрос одной новой реализации
/// без изменения остальной системы.
/// </summary>
public interface IMessageSender
{
    Task SendAsync(IReadOnlyCollection<string> recipients, string subject, string body, CancellationToken ct = default);
}
