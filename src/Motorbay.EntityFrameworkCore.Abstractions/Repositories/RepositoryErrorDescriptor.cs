using Microsoft.EntityFrameworkCore;

namespace Motorbay.EntityFrameworkCore.Abstractions.Repositories;

/// <summary>
/// Used to create <see cref="RepositoryError"/> instances.
/// </summary>
public class RepositoryErrorDescriptor
{
    /// <summary>
    /// Creates a new EntityNotFOund <see cref="RepositoryError"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the entity's key.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="key">The key of the entity that was not found.</param>
    /// <returns>A new EntityNotFound <see cref="RepositoryError"/>.</returns>
    public virtual RepositoryError EntityWithKeyNotFound<TKey, TEntity>(TKey key)
        where TKey : IEquatable<TKey>
        where TEntity : class, IUniqueEntity<TKey>
    {
        return new RepositoryError(
            nameof(EntityWithKeyNotFound), 
            string.Format(Resources.EntityWithKeyNotFound, typeof(TEntity).Name, key)
        );
    }

    /// <summary>
    /// Creates a new NothingWrittenToDatabase <see cref="RepositoryError"/>.
    /// </summary>
    /// <returns>A new NothingWrittenToDatabase <see cref="RepositoryError"/>.</returns>
    public virtual RepositoryError NothingWrittenToDatabase()
    {
        return new RepositoryError(
            nameof(NothingWrittenToDatabase),
            Resources.NothingWrittenToDatabase
        );
    }

    /// <summary>
    /// Creates a new UnexpectedDatabaseWriteCount <see cref="RepositoryError"/>.
    /// </summary>
    /// <param name="expected">The expected number of entities written to the database.</param>
    /// <param name="written">The actual number of entities written to the database.</param>
    /// <returns>A new UnexpectedDatabaseWriteCount <see cref="RepositoryError"/>.</returns>
    public virtual RepositoryError UnexpectedDatabaseWriteCount(int expected, int written)
    {
        return new RepositoryError(
            nameof(UnexpectedDatabaseWriteCount),
            string.Format(Resources.UnexpectedDatabaseWriteCount, expected, written)
        );
    }

    /// <summary>
    /// Creates a new DatabaseConcurrencyFailure <see cref="RepositoryError"/>.
    /// </summary>
    /// <param name="exception">The <see cref="DbUpdateConcurrencyException"/> that occurred.</param>
    /// <returns>A new DatabaseConcurrencyFailure <see cref="RepositoryError"/>.</returns>
    public virtual RepositoryError DatabaseConcurrencyFailure(DbUpdateConcurrencyException exception)
    {
        return new RepositoryError(
            nameof(DatabaseConcurrencyFailure),
            exception.Message,
            exception
        );
    }

    /// <summary>
    /// Creates a new DatabaseUpdateFailure <see cref="RepositoryError"/>.
    /// </summary>
    /// <param name="exception">The <see cref="DbUpdateException"/> that occurred.</param>
    /// <returns>A new DatabaseUpdateFailure <see cref="RepositoryError"/>.</returns>
    /// <remarks>If the <paramref name="exception"/> is a <see cref="DbUpdateConcurrencyException"/>, this method will return a <see cref="DatabaseConcurrencyFailure"/> error.</remarks>
    public virtual RepositoryError DatabaseUpdateFailure(DbUpdateException exception)
    {
        if (exception is DbUpdateConcurrencyException concurrencyException)
        {
            return DatabaseConcurrencyFailure(concurrencyException);
        }

        return new RepositoryError(
            nameof(DatabaseUpdateFailure),
            exception.Message,
            exception
        );
    }
}
