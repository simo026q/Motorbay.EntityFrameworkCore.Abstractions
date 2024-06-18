using Microsoft.EntityFrameworkCore;

namespace Motorbay.EntityFrameworkCore.Abstractions.Contexts;

/// <summary>
/// Base class for database contexts that automatically update timestamps on entities that implement <see cref="ITimestampedEntity"/>.
/// </summary>
/// <inheritdoc />
public abstract class TimestampedDbContext 
    : DbContext
{
    private readonly EntityTimestampUpdater _timestampUpdater;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimestampedDbContext"/> class with specified options and an entity timestamp updater.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    /// <param name="timestampUpdater">The updater responsible for managing entity timestamps.</param>
    protected TimestampedDbContext(DbContextOptions options, EntityTimestampUpdater timestampUpdater)
        : base(options)
    {
        _timestampUpdater = timestampUpdater;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TimestampedDbContext"/> class with specified options and a time provider.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    /// <param name="timeProvider">The time provider to use for setting timestamps.</param>
    protected TimestampedDbContext(DbContextOptions options, TimeProvider timeProvider)
        : base(options)
    {
        _timestampUpdater = new EntityTimestampUpdater(this, timeProvider);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TimestampedDbContext"/> class with specified options and the default system time provider.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    protected TimestampedDbContext(DbContextOptions options)
        : this(options, TimeProvider.System)
    {
    }

    /// <summary>
    /// Obsolete method for updating timestamps of entities in the context.
    /// </summary>
    /// <param name="date">The date to set the timestamps to.</param>
    [Obsolete("This method is obsolete and will be removed in a future version. Override UpdateTimestamps in EntityTimestampUpdater instead and inject it into the constructor.", error: true)]
    protected virtual void UpdateTimestamps(DateTimeOffset date)
    {
        // This method is obsolete and should not be used.
    }
}
