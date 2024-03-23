using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Motorbay.EntityFrameworkCore.Abstractions.Contexts;

/// <summary>
/// Base class for database contexts that automatically update timestamps.
/// </summary>
/// <inheritdoc />
public abstract class TimestampedDbContext 
    : DbContext
{
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimestampedDbContext"/> class.
    /// </summary>
    /// <param name="options">The options for this context. </param>
    /// <param name="timeProvider">The time provider to use for setting timestamps.</param>
    protected TimestampedDbContext(DbContextOptions options, TimeProvider timeProvider)
        : base(options)
    {
        SavingChanges += OnSavingChanges;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TimestampedDbContext"/> class.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    protected TimestampedDbContext(DbContextOptions options)
        : this(options, TimeProvider.System)
    {
    }

    private void OnSavingChanges(object? sender, SavingChangesEventArgs e) 
        => UpdateTimestamps(_timeProvider.GetUtcNow());

    /// <summary>
    /// Updates the timestamps of entities in the context.
    /// </summary>
    /// <param name="date">The date to set the timestamps to.</param>
    protected virtual void UpdateTimestamps(DateTimeOffset date)
    {
        foreach (EntityEntry<ITimestampedEntity> entry in ChangeTracker.Entries<ITimestampedEntity>())
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
