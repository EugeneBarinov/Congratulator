using Congratulator.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Congratulator.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Person> People => Set<Person>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>(entity =>
        {
            entity.ToTable("People");
            entity.HasKey(p => p.Id);

            entity.Property(p => p.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(p => p.LastName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(p => p.Relation)
                .HasMaxLength(100);

            entity.Property(p => p.Notes)
                .HasMaxLength(1000);

            entity.Property(p => p.PhotoFileName)
                .HasMaxLength(260);

            entity.Ignore(p => p.FullName);

            entity.HasIndex(p => p.BirthDate);
        });
    }
}
