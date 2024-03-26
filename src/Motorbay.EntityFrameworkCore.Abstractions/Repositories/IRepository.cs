namespace Motorbay.EntityFrameworkCore.Abstractions.Repositories;

/// <summary>
/// A CRUD repository.
/// </summary>
/// <typeparam name="TKey">The type of the unique identifier.</typeparam>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public interface IRepository<in TKey, TEntity>
    : IReadOnlyRepository<TKey, TEntity>
    where TKey : IEquatable<TKey>
    where TEntity : class, IUniqueEntity<TKey>
{
    /// <summary>
    /// Creates a new <typeparamref name="TEntity"/>.
    /// </summary>
    /// <param name="id">The unique identifier of the <typeparamref name="TEntity"/> to create.</param>
    /// <param name="isTracked"><see langword="true"/> to track the entity; otherwise, <see langword="false"/>.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation, containing the <typeparamref name="TEntity"/> with the specified <typeparamref name="TKey"/> or <see langword="null"/> if not found.</returns>
    Task<TEntity?> GetByIdAsync(TKey id, bool isTracked, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all <typeparamref name="TEntity"/>s.
    /// </summary>
    /// <param name="isTracked"><see langword="true"/> to track the entity; otherwise, <see langword="false"/>.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation, containing a list of <typeparamref name="TEntity"/>s.</returns>
    Task<IList<TEntity>> GetAllAsync(bool isTracked, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new <typeparamref name="TEntity"/>.
    /// </summary>
    /// <param name="entity">The <typeparamref name="TEntity"/> to create.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation, containing a <see cref="RepositoryResult"/> that represents the result of the operation.</returns>
    Task<RepositoryResult> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a range of new <typeparamref name="TEntity"/>s.
    /// </summary>
    /// <param name="entities">The <typeparamref name="TEntity"/>s to create.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation, containing a <see cref="RepositoryResult"/> that represents the result of the operation.</returns>
    Task<RepositoryResult> CreateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing <typeparamref name="TEntity"/>.
    /// </summary>
    /// <param name="entity">The <typeparamref name="TEntity"/> to update.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation, containing a <see cref="RepositoryResult"/> that represents the result of the operation.</returns>
    Task<RepositoryResult> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates a range of existing <typeparamref name="TEntity"/>s.
    /// </summary>
    /// <param name="entities">The <typeparamref name="TEntity"/>s to update.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation, containing a <see cref="RepositoryResult"/> that represents the result of the operation.</returns>
    Task<RepositoryResult> UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes an existing <typeparamref name="TEntity"/>.
    /// </summary>
    /// <param name="entity">The <typeparamref name="TEntity"/> to delete.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation, containing a <see cref="RepositoryResult"/> that represents the result of the operation.</returns>
    Task<RepositoryResult> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a range of existing <typeparamref name="TEntity"/>s.
    /// </summary>
    /// <param name="entities">The <typeparamref name="TEntity"/>s to delete.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation, containing a <see cref="RepositoryResult"/> that represents the result of the operation.</returns>
    Task<RepositoryResult> DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes an existing <typeparamref name="TEntity"/> by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the <typeparamref name="TEntity"/> to delete.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation, containing a <see cref="RepositoryResult"/> that represents the result of the operation.</returns>
    Task<RepositoryResult> DeleteByIdAsync(TKey id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a range of existing <typeparamref name="TEntity"/>s by their unique identifiers.
    /// </summary>
    /// <param name="ids">The unique identifiers of the <typeparamref name="TEntity"/>s to delete.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation, containing a <see cref="RepositoryResult"/> that represents the result of the operation.</returns>
    Task<RepositoryResult> DeleteRangeByIdAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default);
}
