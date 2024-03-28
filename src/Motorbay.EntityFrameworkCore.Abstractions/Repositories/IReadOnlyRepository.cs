namespace Motorbay.EntityFrameworkCore.Abstractions.Repositories;

/// <summary>
/// A read-only repository.
/// </summary>
/// <typeparam name="TKey">The type of the unique identifier.</typeparam>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public interface IReadOnlyRepository<in TKey, TEntity>
    where TKey : IEquatable<TKey>
    where TEntity : class, IUniqueEntity<TKey>
{
    /// <summary>
    /// Gets an <typeparamref name="TEntity"/> by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the <typeparamref name="TEntity"/> to get.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation, containing the <typeparamref name="TEntity"/> with the specified <typeparamref name="TKey"/> or <see langword="null"/> if not found.</returns>
    Task<RepositoryResult<TEntity>> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all <typeparamref name="TEntity"/>s.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation, containing a list of <typeparamref name="TEntity"/>s.</returns>
    Task<RepositoryResult<List<TEntity>>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an <typeparamref name="TEntity"/> with the specified <paramref name="id"/> exists.
    /// </summary>
    /// <param name="id">The unique identifier of the <typeparamref name="TEntity"/> to check.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation, containing <see langword="true"/> if the <typeparamref name="TEntity"/> exists; otherwise, <see langword="false"/>.</returns>
    Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default);
}
