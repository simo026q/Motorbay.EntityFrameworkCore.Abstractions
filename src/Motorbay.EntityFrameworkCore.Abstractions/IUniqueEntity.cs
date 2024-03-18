namespace Motorbay.EntityFrameworkCore.Abstractions;

public interface IUniqueEntity<TKey>
    where TKey : IEquatable<TKey>
{
    TKey Id { get; }
}