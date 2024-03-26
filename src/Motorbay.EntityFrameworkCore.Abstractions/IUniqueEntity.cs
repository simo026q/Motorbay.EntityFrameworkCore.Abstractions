namespace Motorbay.EntityFrameworkCore.Abstractions;

/// <summary>
/// Entity with a unique identifier.
/// </summary>
/// <typeparam name="TKey">The type of the unique identifier.</typeparam>
public interface IUniqueEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Unique identifier for the entity.
    /// </summary>
    TKey Id { get; }
}