using Microsoft.EntityFrameworkCore;

namespace Motorbay.EntityFrameworkCore.Abstractions.Repositories;

/// <summary>
/// Base class for database repositories.
/// </summary>
/// <typeparam name="TKey">The type of the unique identifier.</typeparam>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <param name="context">The <see cref="DbContext"/> to use.</param>
public abstract class DatabaseRepositoryBase<TKey, TEntity>(DbContext context)
    where TKey : IEquatable<TKey>
    where TEntity : class, IUniqueEntity<TKey>
{
    private readonly DbContext _context = context;

    /// <summary>
    /// The <see cref="DbSet{TEntity}"/> of the entity.
    /// </summary>
    protected DbSet<TEntity> Entities => _context.Set<TEntity>();

    /// <summary>
    /// Gets a queryable of the entity.
    /// </summary>
    /// <param name="isTracked"><see langword="true"/> to track the entity; otherwise, <see langword="false"/>.</param>
    /// <returns>A queryable of the entity.</returns>
    protected IQueryable<TEntity> GetQueryable(bool isTracked)
    {
        return isTracked
            ? Entities.AsQueryable()
            : Entities.AsNoTracking();
    }

    /// <summary>
    /// Saves changes to the database.
    /// </summary>
    /// <param name="expectedWritten">The expected number of entities written to the database.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="RepositoryResult"/> that represents the result of the operation.</returns>
    /// <exception cref="OperationCanceledException">The <paramref name="cancellationToken"/> was canceled.</exception>
    protected virtual async Task<RepositoryResult> SaveChangesAsync(int expectedWritten, CancellationToken cancellationToken)
    {
        try
        {
            int written = await _context.SaveChangesAsync(cancellationToken);

            if (written == 0)
            {
                return RepositoryResult.Failure(RepositoryErrorDescriptor.NothingWrittenToDatabase());
            }
            else if (written != expectedWritten)
            {
                return RepositoryResult.Failure(RepositoryErrorDescriptor.UnexpectedDatabaseWriteCount(expectedWritten, written));
            }
            else
            {
                return RepositoryResult.Success;
            }
        }
        catch (DbUpdateException ex)
        {
            return RepositoryResult.Failure(RepositoryErrorDescriptor.DatabaseUpdateFailure(ex));
        }
    }
}