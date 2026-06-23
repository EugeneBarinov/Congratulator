namespace Congratulator.Api.Models;

/// <summary>
/// Человек, чей день рождения отслеживается приложением.
/// Это центральная сущность домена — она не знает ничего ни про БД, ни про HTTP,
/// ни про способ отправки уведомлений (зависимости направлены ОТ инфраструктуры К домену).
/// </summary>
public class Person
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    /// <summary>Дата рождения. Время суток не имеет значения, поэтому DateOnly.</summary>
    public DateOnly BirthDate { get; set; }

    /// <summary>Например: "коллега", "друг", "клиент" — свободное текстовое поле.</summary>
    public string? Relation { get; set; }

    public string? Notes { get; set; }

    /// <summary>Имя файла фотографии на диске (в wwwroot/uploads). Null — фото не загружено.</summary>
    public string? PhotoFileName { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc { get; set; }

    public string FullName => $"{LastName} {FirstName}".Trim();
}
