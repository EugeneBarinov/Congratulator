namespace Congratulator.Api.Options;

/// <summary>
/// Настройки сервиса рассылки из секции "Notifications" appsettings.json.
/// Здесь же конфигурируется расписание и набор адресов получателей,
/// как того требует уровень 5 задания.
/// </summary>
public class NotificationSettings
{
    /// <summary>Включена ли автоматическая рассылка вообще.</summary>
    public bool Enabled { get; set; } = false;

    /// <summary>Время суток ежедневного запуска рассылки в формате "HH:mm" (локальное время сервера).</summary>
    public string DailyRunTime { get; set; } = "09:00";

    /// <summary>В пределах скольких дней ДР включается в дайджест-уведомление.</summary>
    public int UpcomingDaysThreshold { get; set; } = 3;

    /// <summary>Адреса получателей рассылки (e-mail). Список конфигурируется, а не хардкодится.</summary>
    public List<string> Recipients { get; set; } = new();

    public SmtpSettings Smtp { get; set; } = new();
}

/// <summary>
/// Параметры транспорта. Конкретный транспорт (e-mail/мессенджер) выбирается разработчиком —
/// в проекте это e-mail через SMTP, но за счёт абстракции IMessageSender транспорт
/// можно заменить без изменения остальной системы (см. Abstractions/IMessageSender.cs).
/// </summary>
public class SmtpSettings
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public string User { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromAddress { get; set; } = string.Empty;
    public string FromName { get; set; } = "Поздравлятор";
    public bool EnableSsl { get; set; } = true;
}
