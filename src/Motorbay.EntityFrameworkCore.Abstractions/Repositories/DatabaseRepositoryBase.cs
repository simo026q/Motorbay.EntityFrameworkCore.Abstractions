using Microsoft.EntityFrameworkCore;

namespace Motorbay.EntityFrameworkCore.Abstractions.Repositories;

public abstract class DatabaseRepositoryBase<TKey, TEntity>(DbContext context)
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