using Motorbay.EntityFrameworkCore.Abstractions.Repositories;

namespace Motorbay.EntityFrameworkCore.Abstractions.Extensions;

public static class RepositoryExtensions
{
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
