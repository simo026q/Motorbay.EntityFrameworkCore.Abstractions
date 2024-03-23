using Microsoft.EntityFrameworkCore;

namespace Motorbay.EntityFrameworkCore.Abstractions.Repositories;

internal static class RepositoryErrorDescriptor
{
    public static RepositoryError EntityNotFound<TKey, TEntity>(TKey key)
        where TKey : IEquatable<TKey>
        where TEntity : class, IUniqueEntity<TKey>
    {
        return new RepositoryError(
            nameof(EntityNotFound), 
            string.Format(Resources.EntityNotFound, typeof(TEntity).Name, key)
        );
    }

    public static RepositoryError NothingWrittenToDatabase()
    {
        return new RepositoryError(
            nameof(NothingWrittenToDatabase),
            Resources.NothingWrittenToDatabase
        );
    }

    public static RepositoryError UnexpectedDatabaseWriteCount(int expected, int written)
    {
        return new RepositoryError(
            nameof(UnexpectedDatabaseWriteCount),
            string.Format(Resources.UnexpectedDatabaseWriteCount, expected, written)
        );
    }

    public static RepositoryError DatabaseConcurrencyFailure(DbUpdateConcurrencyException exception)
    {
        return new RepositoryError(
            nameof(DatabaseConcurrencyFailure),
            exception.Message,
            exception
        );
    }

    public static RepositoryError DatabaseUpdateFailure(DbUpdateException exception)
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
