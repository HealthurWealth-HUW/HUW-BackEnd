using HealthUrWelath.Application.Common.Caching;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

namespace HealthUrWealth.Infrastructure.Common.Caching;

public sealed class MemoryCacheService : IAppCache
{
    private readonly IMemoryCache _cache;

    private static readonly ConcurrentDictionary<string, bool> _keys = new();

    public MemoryCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public async Task<T> GetOrCreateAsync<T>(
        string key,
        TimeSpan ttl,
        Func<Task<T>> factory)
    {
        if (_cache.TryGetValue(key, out T cached))
            return cached;

        var value = await factory();

        _cache.Set(key, value, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl
        });

        return value;
    }

    public Task RemoveAsync(string key)
    {
        _cache.Remove(key);
        _keys.TryRemove(key, out _);
        return Task.CompletedTask;
    }

    public Task ClearAllAsync()
    {
        foreach (var key in _keys.Keys)
        {
            _cache.Remove(key);
        }

        _keys.Clear();
        return Task.CompletedTask;
    }
}
