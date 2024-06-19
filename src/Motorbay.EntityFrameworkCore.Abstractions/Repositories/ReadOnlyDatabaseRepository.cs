using Microsoft.EntityFrameworkCore;

namespace Motorbay.EntityFrameworkCore.Abstractions.Repositories;

/// <inheritdoc />
public abstract class ReadOnlyDatabaseRepository<TKey, TEntity>(DbContext context, RepositoryErrorDescriptor? errorDescriptor = null)
    : ReadOnlyDatabaseRepository<TKey, TEntity, RepositoryErrorDescriptor>(context, errorDescriptor ?? new()), IReadOnlyRepository<TKey, TEntity>
    where TKey : IEquatable<TKey>
    where TEntity : class, IUniqueEntity<TKey>
{
}

/// <inheritdoc />
public abstract class ReadOnlyDatabaseRepository<TKey, TEntity, TErrorDescriptor>(DbContext context, TErrorDescriptor errorDescriptor)
    : DatabaseRepositoryBase<TKey, TEntity, TErrorDescriptor>(context, errorDescriptor), IReadOnlyRepository<TKey, TEntity>
    where TKey : IEquatable<TKey>
    where TEntity : class, IUniqueEntity<TKey>
    where TErrorDescriptor : RepositoryErrorDescriptor
{
    /// <inheritdoc />
    /// <exception cref="OperationCanceledException">The <paramref name="cancellationToken"/> was cancelled.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
    public virtual Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));

        return Entities.AnyAsync(e => e.Id.Equals(id), cancellationToken);
    }

    /// <inheritdoc />
    /// <exception cref="OperationCanceledException">The <paramref name="cancellationToken"/> was cancelled.</exception>
    public virtual async Task<RepositoryResult<List<TEntity>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        List<TEntity> entities = await GetQueryable(isTracked: false).ToListAsync(cancellationToken);

        return RepositoryResult<List<TEntity>>.Success(entities);
    }

    /// <inheritdoc />
    /// <exception cref="OperationCanceledException">The <paramref name="cancellationToken"/> was cancelled.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
    public virtual async Task<RepositoryResult<TEntity>> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));

        TEntity? entity = await Entities.FindAsync([id], cancellationToken);

        return entity is not null
            ? RepositoryResult<TEntity>.Success(entity)
            : RepositoryResult<TEntity>.Failure(ErrorDescriptor.EntityWithKeyNotFound<TKey, TEntity>(id));
    }
}
