namespace Congratulator.Api.Abstractions;

/// <summary>
/// Сценарий "разослать уведомления о текущих/наступающих ДР". Используется и фоновым
/// сервисом по расписанию (BackgroundServices/BirthdayReminderHostedService.cs), и контроллером
/// для ручного запуска по требованию (Controllers/NotificationsController.cs) — логика не дублируется.
/// </summary>
public interface IBirthdayNotificationService
{
    /// <summary>
    /// Формирует и отправляет дайджест по сконфигурированным адресам, если есть кого поздравлять
    /// и куда отправлять. Возвращает количество людей, попавших в рассылку (0, если рассылка не отправлена).
    /// </summary>
    Task<int> SendDueNotificationsAsync(CancellationToken ct = default);
}
