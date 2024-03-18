using Microsoft.EntityFrameworkCore;
using Motorbay.EntityFrameworkCore.Abstractions.Repositories.Providers;

namespace Motorbay.EntityFrameworkCore.Abstractions.Repositories;

public abstract class RepositoryBase<TKey, TEntity>(DbContext context, IEntityFrameworkKeyProvider<TKey> keyProvider)
    where TKey : IEquatable<TKey>
    where TEntity : class, IUniqueEntity<TKey>
{
    private readonly DbContext _context = context;
    private readonly IEntityFrameworkKeyProvider<TKey> _keyProvider = keyProvider;

    protected DbSet<TEntity> Entities => _context.Set<TEntity>();

    protected IQueryable<TEntity> GetQueryable(bool isTracked)
    {
        return isTracked
            ? Entities.AsQueryable()
            : Entities.AsNoTracking();
    }

    protected async Task<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken)
    {
        object?[]? keys = _keyProvider.GetKeys(id);
        return await Entities.FindAsync(keys, cancellationToken);
    }

    protected virtual async Task<RepositoryResult> SaveChangesAsync(int expectedWritten, CancellationToken cancellationToken)
    {
        try
        {
            int written = await _context.SaveChangesAsync(cancellationToken);

            return written == expectedWritten
                ? RepositoryResult.Success
                : RepositoryResult.Failure(RepositoryError.UnexpectedWriteCount);
        }
        catch (DbUpdateException ex)
        {
            return RepositoryResult.Failure(ex);
        }
    }
}