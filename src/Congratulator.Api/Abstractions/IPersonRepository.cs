using Congratulator.Api.Models;

namespace Congratulator.Api.Abstractions;

/// <summary>
/// Абстракция доступа к данным. Сервисный слой работает с этим интерфейсом и не знает,
/// что под капотом — EF Core + SQLite. Это позволяет, например, подменить реализацию
/// в unit-тестах или сменить БД, не трогая бизнес-логику.
/// </summary>
public interface IPersonRepository
{
    Task<List<Person>> GetAllAsync(CancellationToken ct = default);
    Task<Person?> GetByIdAsync(int id, CancellationToken ct = default);
    Task AddAsync(Person person, CancellationToken ct = default);
    void Remove(Person person);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
