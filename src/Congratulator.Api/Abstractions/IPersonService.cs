using Congratulator.Api.Dtos;
using Microsoft.AspNetCore.Http;

namespace Congratulator.Api.Abstractions;

/// <summary>Прикладные сценарии работы со списком ДР — то, чем пользуется контроллер.</summary>
public interface IPersonService
{
    Task<IReadOnlyList<PersonDto>> GetAllAsync(CancellationToken ct = default);

    /// <summary>Сегодняшние и ближайшие (в пределах настроенного порога) ДР, отсортированные по дате наступления.</summary>
    Task<IReadOnlyList<PersonDto>> GetTodayAndUpcomingAsync(CancellationToken ct = default);

    Task<PersonDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<PersonDto> CreateAsync(PersonUpsertDto dto, CancellationToken ct = default);
    Task<bool> UpdateAsync(int id, PersonUpsertDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);

    Task<PersonDto?> SetPhotoAsync(int id, IFormFile photo, CancellationToken ct = default);
    Task<PersonDto?> RemovePhotoAsync(int id, CancellationToken ct = default);
}
