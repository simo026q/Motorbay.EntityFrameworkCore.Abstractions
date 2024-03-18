namespace Motorbay.EntityFrameworkCore.Abstractions.Repositories;

public interface IReadOnlyRepository<in TKey, TEntity>
    where TKey : IEquatable<TKey>
    where TEntity : class, IUniqueEntity<TKey>
{
    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
    Task<IList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default);
}
