using Microsoft.Extensions.Caching.Memory;

namespace Motorbay.EntityFrameworkCore.Abstractions.Repositories.Caching;

public class CachedRepository<TKey, TEntity>(IRepository<TKey, TEntity> repository, IMemoryCache cache)
    : IRepository<TKey, TEntity>
    where TKey : IEquatable<TKey>
    where TEntity : class, IUniqueEntity<TKey>
{
    private readonly IRepository<TKey, TEntity> _repository = repository;
    private readonly MemoryCacheWrapper<TKey, TEntity> _cache = new(cache);

    public Task<RepositoryResult<TEntity>> GetByIdAsync(TKey id, bool isTracked, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<RepositoryResult<TEntity>> GetByIdAsync(TKey id, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<RepositoryResult<List<TEntity>>> GetAllAsync(bool isTracked, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<RepositoryResult<List<TEntity>>> GetAllAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<RepositoryResult> CreateAsync(TEntity entity, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<RepositoryResult> CreateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<RepositoryResult> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<RepositoryResult> UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<RepositoryResult> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<RepositoryResult> DeleteByIdAsync(TKey id, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<RepositoryResult> DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<RepositoryResult> DeleteRangeByIdAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default) => throw new NotImplementedException();
}
