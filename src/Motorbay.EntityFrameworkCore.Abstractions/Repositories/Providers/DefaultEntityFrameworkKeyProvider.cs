namespace Motorbay.EntityFrameworkCore.Abstractions.Repositories.Providers;

internal sealed class DefaultEntityFrameworkKeyProvider<TKey>
    : IEntityFrameworkKeyProvider<TKey>
    where TKey : IEquatable<TKey>
{
    public static readonly DefaultEntityFrameworkKeyProvider<TKey> Instance = new();

    public object?[]? GetKeys(TKey key) => [key];
}
