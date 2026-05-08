using HealthUrWelath.Application.Common.Caching;

namespace HealthUrWealth.Infrastructure.Common.Caching;

public sealed class RedisCacheService : IAppCache
{
    public Task ClearAllAsync()
    {
        throw new NotImplementedException();
    }

    //private readonly IDatabase _db;

    //public RedisCacheService(IConnectionMultiplexer redis)
    //{
    //    _db = redis.GetDatabase();
    //}

    //public async Task<T> GetOrCreateAsync<T>(
    //    string key,
    //    TimeSpan ttl,
    //    Func<Task<T>> factory)
    //{
    //    var cached = await _db.StringGetAsync(key);

    //    if (cached.HasValue)
    //        return JsonSerializer.Deserialize<T>(cached!)!;

    //    var value = await factory();

    //    var json = JsonSerializer.Serialize(value);

    //    await _db.StringSetAsync(
    //        key,
    //        json,
    //        ttl,
    //        When.Always,
    //        CommandFlags.FireAndForget);

    //    return value;
    //}
    public Task<T> GetOrCreateAsync<T>(string key, TimeSpan ttl, Func<Task<T>> factory)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(string key)
    {
        throw new NotImplementedException();
    }
}
