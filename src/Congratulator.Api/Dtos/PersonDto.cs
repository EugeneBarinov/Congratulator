namespace Congratulator.Api.Dtos;

/// <summary>
/// То, что отдаёт API клиенту. Отдельно от Person — чтобы не "протекать" деталями
/// хранения (например, именем файла на диске) и чтобы отдавать уже посчитанные
/// производные поля (возраст, число дней до ДР), не заставляя фронтенд знать
/// о соглашениях вычисления дат.
/// </summary>
public class PersonDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public string? Relation { get; set; }
    public string? Notes { get; set; }

    /// <summary>Публичный URL фотографии (например, "/uploads/abc.jpg") либо null.</summary>
    public string? PhotoUrl { get; set; }

    public int Age { get; set; }
    public int DaysUntilNextBirthday { get; set; }
    public bool IsToday { get; set; }
    public bool IsUpcoming { get; set; }
    public DateOnly NextOccurrence { get; set; }
}
