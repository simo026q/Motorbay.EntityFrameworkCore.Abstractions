namespace Motorbay.EntityFrameworkCore.Abstractions.Repositories;

public interface IRepository<in TKey, TEntity>
    : IReadOnlyRepository<TKey, TEntity>
    where TKey : IEquatable<TKey>
    where TEntity : class, IUniqueEntity<TKey>
{
    Task<TEntity?> GetByIdAsync(TKey id, bool isTracked, CancellationToken cancellationToken = default);
    Task<IList<TEntity>> GetAllAsync(bool isTracked, CancellationToken cancellationToken = default);
    Task<RepositoryResult> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<RepositoryResult> CreateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    Task<RepositoryResult> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<RepositoryResult> UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    Task<RepositoryResult> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<RepositoryResult> DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    Task<RepositoryResult> DeleteByIdAsync(TKey id, CancellationToken cancellationToken = default);
    Task<RepositoryResult> DeleteRangeByIdAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default);
}
