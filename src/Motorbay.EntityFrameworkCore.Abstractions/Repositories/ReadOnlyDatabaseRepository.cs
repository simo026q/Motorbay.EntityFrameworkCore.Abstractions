using Microsoft.EntityFrameworkCore;

namespace Motorbay.EntityFrameworkCore.Abstractions.Repositories;

public abstract class ReadOnlyDatabaseRepository<TKey, TEntity>(DbContext context)
    : IReadOnlyRepository<TKey, TEntity>
    where TKey : IEquatable<TKey>
    where TEntity : class, IUniqueEntity<TKey>
{
    private readonly DbContext _context = context;

    protected DbSet<TEntity> Entities => _context.Set<TEntity>();

    protected IQueryable<TEntity> GetQueryable(bool isTracked)
    {
        return isTracked
            ? Entities.AsQueryable()
            : Entities.AsNoTracking();
    }

    public virtual Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default) 
        => Entities.AnyAsync(e => e.Id.Equals(id), cancellationToken);

    public virtual async Task<IList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default) 
        => await GetQueryable(isTracked: false).ToListAsync(cancellationToken);

    public virtual async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default) 
        => await Entities.FindAsync([id], cancellationToken);

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
