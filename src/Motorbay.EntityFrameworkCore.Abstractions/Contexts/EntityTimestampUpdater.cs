using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Motorbay.EntityFrameworkCore.Abstractions.Contexts;

/// <summary>
/// Handles updating the timestamps of entities that implement <see cref="ITimestampedEntity"/> in a <see cref="DbContext"/>.
/// </summary>
/// <remarks>If possible, it's recommended to use <see cref="TimestampedDbContext"/> instead of this class.</remarks>
public class EntityTimestampUpdater
{
    private readonly DbContext _context;
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityTimestampUpdater"/> class with a specified <see cref="TimeProvider"/>.
    /// </summary>
    /// <param name="context">The <see cref="DbContext"/> to update the timestamps of.</param>
    /// <param name="timeProvider">The time provider to use for setting timestamps.</param>
    public EntityTimestampUpdater(DbContext context, TimeProvider timeProvider)
    {
        _context = context;
        _timeProvider = timeProvider;
        _context.SavingChanges += OnSavingChanges;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityTimestampUpdater"/> class using the default system time provider.
    /// </summary>
    /// <param name="context">The <see cref="DbContext"/> to update the timestamps of.</param>
    public EntityTimestampUpdater(DbContext context)
        : this(context, TimeProvider.System)
    {
    }

    /// <summary>
    /// Event handler for the <see cref="DbContext.SavingChanges"/> event that updates timestamps before changes are saved.
    /// </summary>
    private void OnSavingChanges(object? sender, SavingChangesEventArgs e)
        => UpdateTimestamps(_timeProvider.GetUtcNow());

    /// <summary>
    /// Updates the timestamps of entities in the context.
    /// </summary>
    /// <param name="timestamp">The timestamp to set for the entities.</param>
    protected virtual void UpdateTimestamps(DateTimeOffset timestamp)
    {
        foreach (EntityEntry<ITimestampedEntity> entry in _context.ChangeTracker.Entries<ITimestampedEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = timestamp;
                    entry.Entity.UpdatedAt = timestamp;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = timestamp;
                    break;
            }
        }
    }
}
