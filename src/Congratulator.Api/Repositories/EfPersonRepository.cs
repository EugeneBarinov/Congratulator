using Congratulator.Api.Abstractions;
using Congratulator.Api.Data;
using Congratulator.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Congratulator.Api.Repositories;

public class EfPersonRepository : IPersonRepository
{
    private readonly AppDbContext _db;

    public EfPersonRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Person>> GetAllAsync(CancellationToken ct = default)
        => await _db.People.AsNoTracking().ToListAsync(ct);

    public async Task<Person?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _db.People.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task AddAsync(Person person, CancellationToken ct = default)
        => await _db.People.AddAsync(person, ct);

    public void Remove(Person person)
        => _db.People.Remove(person);

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
