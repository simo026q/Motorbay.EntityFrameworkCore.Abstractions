﻿using Microsoft.EntityFrameworkCore;

namespace Motorbay.EntityFrameworkCore.Abstractions.Repositories;

/// <inheritdoc />
public abstract class ReadOnlyDatabaseRepository<TKey, TEntity>(DbContext context)
    : DatabaseRepositoryBase<TKey, TEntity>(context), IReadOnlyRepository<TKey, TEntity>
    where TKey : IEquatable<TKey>
    where TEntity : class, IUniqueEntity<TKey>
{
    /// <inheritdoc />
    public virtual Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default) 
        => Entities.AnyAsync(e => e.Id.Equals(id), cancellationToken);

    /// <inheritdoc />
    public virtual async Task<IList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default) 
        => await GetQueryable(isTracked: false).ToListAsync(cancellationToken);

    /// <inheritdoc />
    public virtual async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default) 
        => await Entities.FindAsync([id], cancellationToken);
}
