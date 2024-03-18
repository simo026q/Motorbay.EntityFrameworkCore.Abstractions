using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Motorbay.EntityFrameworkCore.Abstractions.Contexts;

public abstract class TimestampedDbContext 
    : DbContext
{
    private readonly TimeProvider _timeProvider;

    protected TimestampedDbContext(DbContextOptions options, TimeProvider timeProvider)
        : base(options)
    {
        SavingChanges += OnSavingChanges;
        _timeProvider = timeProvider;
    }

    protected TimestampedDbContext(DbContextOptions options)
        : this(options, TimeProvider.System)
    {
    }

    private void OnSavingChanges(object? sender, SavingChangesEventArgs e) 
        => UpdateTimestamps(_timeProvider.GetUtcNow());

    protected virtual void UpdateTimestamps(DateTimeOffset date)
    {
        IEnumerable<EntityEntry<ITimestampedEntity>> entries = ChangeTracker.Entries<ITimestampedEntity>();

        foreach (EntityEntry<ITimestampedEntity> entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = date;
                    entry.Entity.UpdatedAt = date;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = date;
                    break;
            }
        }
    }
}
