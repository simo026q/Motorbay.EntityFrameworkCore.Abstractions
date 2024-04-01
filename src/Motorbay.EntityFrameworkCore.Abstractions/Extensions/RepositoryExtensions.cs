using Motorbay.EntityFrameworkCore.Abstractions.Repositories;

namespace Motorbay.EntityFrameworkCore.Abstractions.Extensions;

/// <summary>
/// Extension methods for <see cref="IRepository{TKey, TEntity}"/>.
/// </summary>
public static class RepositoryExtensions
{
    /// <summary>
    /// Tries to create an entity if it does not exist.
    /// </summary>
    /// <typeparam name="TKey">The type of the unique identifier.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="repository">The repository to create the entity in.</param>
    /// <param name="key">The unique identifier of the entity to create.</param>
    /// <param name="entityFactory">A factory function to create the entity.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation, containing a <see cref="RepositoryResult"/> that represents the result of the operation.</returns>
    public static async Task<RepositoryResult> TryCreateAsync<TKey, TEntity>(this IRepository<TKey, TEntity> repository, TKey key, Func<TKey, TEntity> entityFactory, CancellationToken cancellationToken = default)
        where TKey : IEquatable<TKey>
        where TEntity : class, IUniqueEntity<TKey>
    {
        ArgumentNullException.ThrowIfNull(repository, nameof(repository));
        ArgumentNullException.ThrowIfNull(key, nameof(key));
        ArgumentNullException.ThrowIfNull(entityFactory, nameof(entityFactory));

        if (await repository.ExistsAsync(key, cancellationToken))
        {
            return RepositoryResult.Success;
        }

        TEntity entity = entityFactory(key);

        return await repository.CreateAsync(entity, cancellationToken);
    }

    /// <summary>
    /// Gets a <typeparamref name="TEntity"/> by its unique identifier or creates it if it does not exist.
    /// </summary>
    /// <typeparam name="TKey">The type of the unique identifier.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="repository">The repository to get or create the entity in.</param>
    /// <param name="key">The unique identifier of the entity to get or create.</param>
    /// <param name="entityFactory">A factory function to create the entity.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation, containing a <see cref="RepositoryResult{TEntity}"/> that represents the result of the operation.</returns>
    public static async Task<RepositoryResult<TEntity>> GetOrCreateAsync<TKey, TEntity>(this IRepository<TKey, TEntity> repository, TKey key, Func<TKey, TEntity> entityFactory, CancellationToken cancellationToken = default)
        where TKey : IEquatable<TKey>
        where TEntity : class, IUniqueEntity<TKey>
    {
        ArgumentNullException.ThrowIfNull(repository, nameof(repository));
        ArgumentNullException.ThrowIfNull(key, nameof(key));
        ArgumentNullException.ThrowIfNull(entityFactory, nameof(entityFactory));

        RepositoryResult<TEntity> getByIdResult = await repository.GetByIdAsync(key, cancellationToken);

        if (getByIdResult.IsSuccessful)
        {
            return getByIdResult;
        }

        TEntity entity = entityFactory(key);

        RepositoryResult createResult = await repository.CreateAsync(entity, cancellationToken);

        return createResult.IsSuccessful 
            ? RepositoryResult<TEntity>.Success(entity) 
            : RepositoryResult<TEntity>.Failure(createResult.Errors);
    }

    /// <summary>
    /// Creates or updates an entity.
    /// </summary>
    /// <typeparam name="TKey">The type of the unique identifier.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="repository">The repository to create or update the entity in.</param>
    /// <param name="entity">The entity to create or update.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation, containing a <see cref="RepositoryResult"/> that represents the result of the operation.</returns>
    public static async Task<RepositoryResult> CreateOrUpdateAsync<TKey, TEntity>(this IRepository<TKey, TEntity> repository, TEntity entity, CancellationToken cancellationToken = default)
        where TKey : IEquatable<TKey>
        where TEntity : class, IUniqueEntity<TKey>
    {
        ArgumentNullException.ThrowIfNull(repository, nameof(repository));
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        if (!entity.Id.Equals(default) && await repository.ExistsAsync(entity.Id, cancellationToken))
        {
            return await repository.UpdateAsync(entity, cancellationToken);
        }

        return await repository.CreateAsync(entity, cancellationToken);
    }
}
