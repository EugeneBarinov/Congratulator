using System.ComponentModel.DataAnnotations;

namespace Congratulator.Api.Dtos;

/// <summary>Данные, которые присылает клиент при создании/редактировании записи (без фото — оно отдельным эндпоинтом).</summary>
public class PersonUpsertDto
{
    [Required(ErrorMessage = "Имя обязательно")]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Фамилия обязательна")]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Дата рождения обязательна")]
    public DateOnly BirthDate { get; set; }

    [MaxLength(100)]
    public string? Relation { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }
}
