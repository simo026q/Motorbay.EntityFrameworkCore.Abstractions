using Microsoft.EntityFrameworkCore;

namespace Motorbay.EntityFrameworkCore.Abstractions.UnitTests.Repositories;

public class TestDbContext(DbContextOptions options)
    : DbContext(options)
{
    public DbSet<GuidUniqueEntity> GuidUniqueEntities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GuidUniqueEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }
}
