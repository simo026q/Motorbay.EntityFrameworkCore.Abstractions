using System.Runtime.CompilerServices;

namespace Motorbay.EntityFrameworkCore.Abstractions.Repositories.Providers;

public sealed class TupleEntityFrameworkKeyProvider<TKey>
    : IEntityFrameworkKeyProvider<TKey>
    where TKey : ITuple, IEquatable<TKey>
{
    public static readonly TupleEntityFrameworkKeyProvider<TKey> Instance = new();

    public object?[]? GetKeys(TKey key)
    {
        var keys = new object?[key.Length];
        for (int i = 0; i < key.Length; i++)
        {
            keys[i] = key[i];
        }

        return keys;
    }
}
