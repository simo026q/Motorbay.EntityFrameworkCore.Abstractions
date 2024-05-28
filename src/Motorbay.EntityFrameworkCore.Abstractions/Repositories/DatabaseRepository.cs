using Microsoft.EntityFrameworkCore;
using Motorbay.EntityFrameworkCore.Abstractions.Extensions;

namespace Motorbay.EntityFrameworkCore.Abstractions.Repositories;

/// <inheritdoc />
public abstract class DatabaseRepository<TKey, TEntity>(DbContext context, RepositoryErrorDescriptor? errorDescriptor = null)
    : ReadOnlyDatabaseRepository<TKey, TEntity>(context, errorDescriptor), IRepository<TKey, TEntity>
    where TKey : IEquatable<TKey>
    where TEntity : class, IUniqueEntity<TKey>
{
    /// <inheritdoc />
    /// <exception cref="OperationCanceledException">The <paramref name="cancellationToken"/> was cancelled.</exception>
    public virtual async Task<RepositoryResult<List<TEntity>>> GetAllAsync(bool isTracked, CancellationToken cancellationToken = default)
    {
        List<TEntity> entities = await GetQueryable(isTracked).ToListAsync(cancellationToken);

        return RepositoryResult<List<TEntity>>.Success(entities);
    }

    /// <inheritdoc />
    /// <exception cref="OperationCanceledException">The <paramref name="cancellationToken"/> was cancelled.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
    public virtual async Task<RepositoryResult<TEntity>> GetByIdAsync(TKey id, bool isTracked, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));

        TEntity? entity = isTracked
            ? await Entities.FindAsync([id], cancellationToken)
            : await GetQueryable(isTracked).FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);

        return entity is not null
            ? RepositoryResult<TEntity>.Success(entity)
            : RepositoryResult<TEntity>.Failure(ErrorDescriptor.EntityWithKeyNotFound<TKey, TEntity>(id));
    }

    /// <inheritdoc />
    /// <exception cref="OperationCanceledException">The <paramref name="cancellationToken"/> was cancelled.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="entity"/> is <see langword="null"/>.</exception>
    public virtual async Task<RepositoryResult> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        await Entities.AddAsync(entity, cancellationToken);

        return await SaveChangesAsync(expectedWritten: 1, cancellationToken);
    }

    /// <inheritdoc />
    /// <exception cref="OperationCanceledException">The <paramref name="cancellationToken"/> was cancelled.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="entities"/> is <see langword="null"/>.</exception>
    public virtual async Task<RepositoryResult> CreateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities, nameof(entities));

        if (entities is ICollection<TEntity> collection)
        {
            await Entities.AddRangeAsync(entities, cancellationToken);
            return await SaveChangesAsync(expectedWritten: collection.Count, cancellationToken);
        }

        int count = 0;
        foreach (TEntity entity in entities)
        {
            await Entities.AddAsync(entity, cancellationToken);
            count++;
        }

        return await SaveChangesAsync(expectedWritten: count, cancellationToken);
    }

    /// <inheritdoc />
    /// <exception cref="OperationCanceledException">The <paramref name="cancellationToken"/> was cancelled.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="entity"/> is <see langword="null"/>.</exception>
    public virtual Task<RepositoryResult> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        Entities.Remove(entity);

        return SaveChangesAsync(expectedWritten: 1, cancellationToken);
    }

    /// <inheritdoc />
    /// <exception cref="OperationCanceledException">The <paramref name="cancellationToken"/> was cancelled.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
    public virtual async Task<RepositoryResult> DeleteByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));

        TEntity? entity = await GetByIdAsync(id, isTracked: true, cancellationToken);

        if (entity is null)
        {
            return RepositoryResult.Failure(ErrorDescriptor.EntityWithKeyNotFound<TKey, TEntity>(id));
        }

        return await DeleteAsync(entity, cancellationToken);
    }

    /// <inheritdoc />
    /// <exception cref="OperationCanceledException">The <paramref name="cancellationToken"/> was cancelled.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="entities"/> is <see langword="null"/>.</exception>
    public virtual Task<RepositoryResult> DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities, nameof(entities));

        if (entities is ICollection<TEntity> collection)
        {
            Entities.RemoveRange(entities);
            return SaveChangesAsync(expectedWritten: collection.Count, cancellationToken);
        }

        int count = 0;
        foreach (TEntity entity in entities)
        {
            count++;
            Entities.Remove(entity);
        }

        return SaveChangesAsync(expectedWritten: count, cancellationToken);
    }

    /// <inheritdoc />
    /// <exception cref="OperationCanceledException">The <paramref name="cancellationToken"/> was cancelled.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="ids"/> is <see langword="null"/>.</exception>
    public virtual async Task<RepositoryResult> DeleteRangeByIdAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(ids, nameof(ids));

        List<RepositoryError> errors = [];

        int updateCount = 0;
        int totalCount = 0;
        foreach (TKey id in ids)
        {
            TEntity? entity = await GetByIdAsync(id, isTracked: true, cancellationToken);

            if (entity is not null)
            {
                updateCount++;
                Entities.Remove(entity);
            }
            else
            {
                errors.Add(ErrorDescriptor.EntityWithKeyNotFound<TKey, TEntity>(id));
            }

            totalCount++;
        }

        RepositoryResult getEntityResult;
        if (errors.Count == 0)
        {
            getEntityResult = RepositoryResult.Success;
        }
        else if (errors.Count == totalCount)
        {
            getEntityResult = RepositoryResult.Failure(errors);
        }
        else
        {
            getEntityResult = RepositoryResult.PartialSuccess(errors);
        }

        if (updateCount > 0)
        {
            RepositoryResult saveResult = await SaveChangesAsync(expectedWritten: updateCount, cancellationToken);

            return getEntityResult.Aggregate(saveResult);
        }
        else
        {
            return getEntityResult;
        }
    }

    /// <inheritdoc />
    /// <exception cref="OperationCanceledException">The <paramref name="cancellationToken"/> was cancelled.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="entity"/> is <see langword="null"/>.</exception>
    public virtual Task<RepositoryResult> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        Entities.Update(entity);

        return SaveChangesAsync(expectedWritten: 1, cancellationToken);
    }

    /// <inheritdoc />
    /// <exception cref="OperationCanceledException">The <paramref name="cancellationToken"/> was cancelled.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="entities"/> is <see langword="null"/>.</exception>
    public virtual Task<RepositoryResult> UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities, nameof(entities));

        if (entities is ICollection<TEntity> collection)
        {
            Entities.UpdateRange(entities);
            return SaveChangesAsync(expectedWritten: collection.Count, cancellationToken);
        }

        int count = 0;
        foreach (TEntity entity in entities)
        {
            count++;
            Entities.Update(entity);
        }

        return SaveChangesAsync(expectedWritten: count, cancellationToken);
    }
}
