using HealthUrWelath.Application.Common.Caching;

namespace HealthUrWealth.Infrastructure.Common.Caching;

public sealed class RedisCacheService : IAppCache
{
    public Task<T?> GetOrCreateAsync<T>(string key, TimeSpan ttl, Func<Task<T>> factory)
    {
        throw new NotImplementedException();
    }

    public Task<T?> GetOrCreateAsync<T>(string featureName, string key, Func<Task<T>> factory)
    {
        throw new NotImplementedException();
    }

    public bool IsFeatureEnabled(string featureName)
    {
        throw new NotImplementedException();
    }

    public Task ClearAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(string key)
    {
        throw new NotImplementedException();
    }
}
