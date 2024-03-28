using Microsoft.EntityFrameworkCore;

namespace Motorbay.EntityFrameworkCore.Abstractions.Repositories;

/// <inheritdoc />
public abstract class ReadOnlyDatabaseRepository<TKey, TEntity>(DbContext context)
    : DatabaseRepositoryBase<TKey, TEntity>(context), IReadOnlyRepository<TKey, TEntity>
    where TKey : IEquatable<TKey>
    where TEntity : class, IUniqueEntity<TKey>
{
    /// <inheritdoc />
    /// <exception cref="OperationCanceledException">The <paramref name="cancellationToken"/> was cancelled.</exception>
    public virtual Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default) 
        => Entities.AnyAsync(e => e.Id.Equals(id), cancellationToken);

    /// <inheritdoc />
    /// <exception cref="OperationCanceledException">The <paramref name="cancellationToken"/> was cancelled.</exception>
    public virtual async Task<RepositoryResult<List<TEntity>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        List<TEntity> entities = await GetQueryable(isTracked: false).ToListAsync(cancellationToken);

        return RepositoryResult<List<TEntity>>.Success(entities);
    }

    /// <inheritdoc />
    /// <exception cref="OperationCanceledException">The <paramref name="cancellationToken"/> was cancelled.</exception>
    public virtual async Task<RepositoryResult<TEntity>> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        TEntity? entity = await Entities.FindAsync([id], cancellationToken);

        return entity is not null
            ? RepositoryResult<TEntity>.Success(entity)
            : RepositoryResult<TEntity>.Failure(RepositoryErrorDescriptor.EntityNotFound<TKey, TEntity>(id));
    }
}
