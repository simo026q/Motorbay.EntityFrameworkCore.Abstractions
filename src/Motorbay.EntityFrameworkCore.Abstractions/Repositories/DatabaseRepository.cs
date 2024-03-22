using Microsoft.EntityFrameworkCore;

namespace Motorbay.EntityFrameworkCore.Abstractions.Repositories;

public abstract class DatabaseRepository<TKey, TEntity>(DbContext context)
    : ReadOnlyDatabaseRepository<TKey, TEntity>(context), IRepository<TKey, TEntity>
    where TKey : IEquatable<TKey>
    where TEntity : class, IUniqueEntity<TKey>
{
    public virtual async Task<IList<TEntity>> GetAllAsync(bool isTracked, CancellationToken cancellationToken = default)
        => await GetQueryable(isTracked).ToListAsync(cancellationToken);

    public virtual async Task<TEntity?> GetByIdAsync(TKey id, bool isTracked, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));

        return isTracked
            ? await Entities.FindAsync([id], cancellationToken)
            : await GetQueryable(isTracked: false).FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);
    }

    public virtual async Task<RepositoryResult> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        await Entities.AddAsync(entity, cancellationToken);

        return await SaveChangesAsync(expectedWritten: 1, cancellationToken);
    }

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

    public virtual Task<RepositoryResult> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        Entities.Remove(entity);

        return SaveChangesAsync(expectedWritten: 1, cancellationToken);
    }

    public virtual async Task<RepositoryResult> DeleteByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));

        TEntity? entity = await GetByIdAsync(id, isTracked: true, cancellationToken);

        if (entity is null)
        {
            return RepositoryResult.Failure(RepositoryError.EntityNotFound(id.ToString()));
        }

        return await DeleteAsync(entity, cancellationToken);
    }

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

    public virtual async Task<RepositoryResult> DeleteRangeByIdAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(ids, nameof(ids));

        List<RepositoryError> errors = [];

        int count = 0;
        int totalCount = 0;
        foreach (TKey id in ids)
        {
            TEntity? entity = await GetByIdAsync(id, isTracked: true, cancellationToken);

            if (entity is not null)
            {
                count++;
                Entities.Remove(entity);
            }
            else
            {
                errors.Add(RepositoryError.EntityNotFound(id.ToString()));
            }

            totalCount++;
        }

        RepositoryResult getResult;
        if (errors.Count == 0)
        {
            getResult = RepositoryResult.Success;
        }
        else if (errors.Count == totalCount)
        {
            getResult = RepositoryResult.Failure(errors);
        }
        else
        {
            getResult = RepositoryResult.PartialSuccess(errors);
        }

        RepositoryResult saveResult = await SaveChangesAsync(expectedWritten: count, cancellationToken);

        return getResult.Aggregate(saveResult);
    }

    public virtual Task<RepositoryResult> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        Entities.Update(entity);

        return SaveChangesAsync(expectedWritten: 1, cancellationToken);
    }

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
