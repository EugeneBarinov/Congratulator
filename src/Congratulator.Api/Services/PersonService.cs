using Congratulator.Api.Abstractions;
using Congratulator.Api.Common;
using Congratulator.Api.Dtos;
using Congratulator.Api.Models;
using Congratulator.Api.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Congratulator.Api.Services;

public class PersonService : IPersonService
{
    private readonly IPersonRepository _repository;
    private readonly IPhotoStorage _photoStorage;
    private readonly IOptionsMonitor<BirthdaySettings> _settings;
    private readonly TimeProvider _timeProvider;

    public PersonService(
        IPersonRepository repository,
        IPhotoStorage photoStorage,
        IOptionsMonitor<BirthdaySettings> settings,
        TimeProvider timeProvider)
    {
        _repository = repository;
        _photoStorage = photoStorage;
        _settings = settings;
        _timeProvider = timeProvider;
    }

    private DateOnly Today => DateOnly.FromDateTime(_timeProvider.GetLocalNow().DateTime);

    private int UpcomingThreshold => _settings.CurrentValue.UpcomingDaysThreshold;

    public async Task<IReadOnlyList<PersonDto>> GetAllAsync(CancellationToken ct = default)
    {
        var people = await _repository.GetAllAsync(ct);
        var today = Today;

        return people
            .Select(p => p.ToDto(today, UpcomingThreshold))
            .OrderBy(p => p.DaysUntilNextBirthday)
            .ThenBy(p => p.LastName)
            .ToList();
    }

    public async Task<IReadOnlyList<PersonDto>> GetTodayAndUpcomingAsync(CancellationToken ct = default)
    {
        var all = await GetAllAsync(ct);
        return all.Where(p => p.IsToday || p.IsUpcoming).ToList();
    }

    public async Task<PersonDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var person = await _repository.GetByIdAsync(id, ct);
        return person?.ToDto(Today, UpcomingThreshold);
    }

    public async Task<PersonDto> CreateAsync(PersonUpsertDto dto, CancellationToken ct = default)
    {
        var person = new Person
        {
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            BirthDate = dto.BirthDate,
            Relation = NullIfEmpty(dto.Relation),
            Notes = NullIfEmpty(dto.Notes),
            CreatedAtUtc = DateTime.UtcNow,
        };

        await _repository.AddAsync(person, ct);
        await _repository.SaveChangesAsync(ct);

        return person.ToDto(Today, UpcomingThreshold);
    }

    public async Task<bool> UpdateAsync(int id, PersonUpsertDto dto, CancellationToken ct = default)
    {
        var person = await _repository.GetByIdAsync(id, ct);
        if (person is null)
        {
            return false;
        }

        person.FirstName = dto.FirstName.Trim();
        person.LastName = dto.LastName.Trim();
        person.BirthDate = dto.BirthDate;
        person.Relation = NullIfEmpty(dto.Relation);
        person.Notes = NullIfEmpty(dto.Notes);
        person.UpdatedAtUtc = DateTime.UtcNow;

        await _repository.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var person = await _repository.GetByIdAsync(id, ct);
        if (person is null)
        {
            return false;
        }

        if (person.PhotoFileName is not null)
        {
            _photoStorage.Delete(person.PhotoFileName);
        }

        _repository.Remove(person);
        await _repository.SaveChangesAsync(ct);
        return true;
    }

    public async Task<PersonDto?> SetPhotoAsync(int id, IFormFile photo, CancellationToken ct = default)
    {
        var person = await _repository.GetByIdAsync(id, ct);
        if (person is null)
        {
            return null;
        }

        var newFileName = await _photoStorage.SaveAsync(photo, ct);

        if (person.PhotoFileName is not null)
        {
            _photoStorage.Delete(person.PhotoFileName);
        }

        person.PhotoFileName = newFileName;
        person.UpdatedAtUtc = DateTime.UtcNow;

        await _repository.SaveChangesAsync(ct);
        return person.ToDto(Today, UpcomingThreshold);
    }

    public async Task<PersonDto?> RemovePhotoAsync(int id, CancellationToken ct = default)
    {
        var person = await _repository.GetByIdAsync(id, ct);
        if (person is null)
        {
            return null;
        }

        if (person.PhotoFileName is not null)
        {
            _photoStorage.Delete(person.PhotoFileName);
            person.PhotoFileName = null;
            person.UpdatedAtUtc = DateTime.UtcNow;
            await _repository.SaveChangesAsync(ct);
        }

        return person.ToDto(Today, UpcomingThreshold);
    }

    private static string? NullIfEmpty(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
