using Microsoft.Extensions.Caching.Memory;
using System.Text;

namespace Motorbay.EntityFrameworkCore.Abstractions.Repositories.Caching;

public class MemoryCacheWrapper<TKey, TEntity>(IMemoryCache cache)
    where TKey : IEquatable<TKey>
    where TEntity : class, IUniqueEntity<TKey>
{
    private static readonly string EntityName;
    private static readonly long EntrySize;

    private readonly IMemoryCache _cache = cache;

    static MemoryCacheWrapper()
    {
        Type entityType = typeof(TEntity);

        EntityName = entityType.Name;
        EntrySize = MemoryHelper.GetSize(entityType);
    }

    private string GetCacheKey(string group, string? key)
    {
        StringBuilder builder = new();

        builder.Append(EntityName);
        builder.Append('_');
        builder.Append(group);

        if (key is not null)
        {
            builder.Append('_');
            builder.Append(key);
        }

        return builder.ToString();
    }

    private static long CalculateSize(RepositoryResult result, int count)
    {
        return result.Succeeded 
            ? EntrySize * count 
            : 0;
    }

    public RepositoryResult<TEntity> Set(string group, string? key, RepositoryResult<TEntity> result)
    {
        ICacheEntry entry = _cache.CreateEntry(GetCacheKey(group, key));

        entry.Size = CalculateSize(result, count: 1);
        entry.Value = result;

        if (!result.Succeeded)
        {
            entry.Priority = CacheItemPriority.Low;
        }

        return result;
    }

    public RepositoryResult<ICollection<TEntity>> Set(string group, string? key, RepositoryResult<ICollection<TEntity>> result)
    {
        ICacheEntry entry = _cache.CreateEntry(GetCacheKey(group, key));

        int count = result.GetValueOrDefault()?.Count ?? 0;

        entry.Size = CalculateSize(result, count);
        entry.Value = result;

        if (!result.Succeeded)
        {
            entry.Priority = CacheItemPriority.Low;
        }

        return result;
    }
}
