namespace Motorbay.EntityFrameworkCore.Abstractions.Repositories;

public interface IEntityFrameworkKeyProvider<TKey>
    where TKey : IEquatable<TKey>
{
    object?[]? GetKeys(TKey key);
}
