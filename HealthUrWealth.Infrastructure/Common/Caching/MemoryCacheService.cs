using HealthUrWelath.Application.Common.Caching;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace HealthUrWealth.Infrastructure.Common.Caching;

public sealed class MemoryCacheService : IAppCache
{
    private readonly IMemoryCache _cache;
    private readonly ICacheConfigurationManager _configManager;
    private readonly ILogger<MemoryCacheService> _logger;
    private static readonly ConcurrentDictionary<string, bool> _keys = new();

    public MemoryCacheService(
        IMemoryCache cache,
        ICacheConfigurationManager configManager,
        ILogger<MemoryCacheService> logger)
    {
        _cache = cache;
        _configManager = configManager;
        _logger = logger;
    }

    public async Task<T?> GetOrCreateAsync<T>(
        string key,
        TimeSpan ttl,
        Func<Task<T>> factory)
    {
        if (!_configManager.IsCacheEnabled())
        {
            _logger.LogDebug("Cache disabled globally, bypassing cache for key: {Key}", key);
            return await factory();
        }

        if (_cache.TryGetValue(key, out T? cached))
        {
            _logger.LogDebug("Cache hit for key: {Key}", key);
            return cached;
        }

        _logger.LogDebug("Cache miss for key: {Key}, calling factory", key);
        var value = await factory();

        _cache.Set(key, value, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl
        });

        _keys.TryAdd(key, true);
        return value;
    }

    public async Task<T?> GetOrCreateAsync<T>(
        string featureName,
        string key,
        Func<Task<T>> factory)
    {
        if (!_configManager.IsFeatureEnabled(featureName))
        {
            _logger.LogDebug("Cache feature '{Feature}' disabled, bypassing cache for key: {Key}", 
                featureName, key);
            return await factory();
        }

        var ttl = _configManager.GetTtl(featureName);
        return await GetOrCreateAsync(key, ttl, factory);
    }

    public bool IsFeatureEnabled(string featureName) 
        => _configManager.IsFeatureEnabled(featureName);

    public Task RemoveAsync(string key)
    {
        _cache.Remove(key);
        _keys.TryRemove(key, out _);
        _logger.LogDebug("Cache cleared for key: {Key}", key);
        return Task.CompletedTask;
    }

    public Task ClearAllAsync()
    {
        foreach (var key in _keys.Keys)
        {
            _cache.Remove(key);
        }

        _keys.Clear();
        _logger.LogInformation("All cache entries cleared");
        return Task.CompletedTask;
    }
}
