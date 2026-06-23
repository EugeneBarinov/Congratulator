namespace Congratulator.Api.Options;

/// <summary>Настройки из секции "Birthdays" appsettings.json.</summary>
public class BirthdaySettings
{
    /// <summary>В пределах скольких дней ДР считается "ближайшим" (для главной страницы).</summary>
    public int UpcomingDaysThreshold { get; set; } = 7;
}
