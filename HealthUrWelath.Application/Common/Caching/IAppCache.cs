namespace HealthUrWelath.Application.Common.Caching
{
    public interface IAppCache
    {
        Task<T> GetOrCreateAsync<T>(
            string key,
            TimeSpan ttl,
            Func<Task<T>> factory);

        Task RemoveAsync(string key);

        Task ClearAllAsync();
    }
}
