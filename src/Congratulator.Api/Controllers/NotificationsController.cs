using Congratulator.Api.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Congratulator.Api.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly IBirthdayNotificationService _notificationService;

    public NotificationsController(IBirthdayNotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    /// <summary>
    /// Запускает рассылку дайджеста вне расписания (полезно для демонстрации/проверки
    /// настроек SMTP и получателей, не дожидаясь сконфигурированного времени).
    /// </summary>
    [HttpPost("run-now")]
    public async Task<IActionResult> RunNow(CancellationToken ct)
    {
        var count = await _notificationService.SendDueNotificationsAsync(ct);
        return Ok(new { notifiedCount = count });
    }
}
