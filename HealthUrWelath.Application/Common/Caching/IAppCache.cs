namespace HealthUrWelath.Application.Common.Caching
{
    public interface IAppCache
    {
        /// <summary>
        /// Gets or creates a cached value with explicit TTL configuration
        /// </summary>
        Task<T?> GetOrCreateAsync<T>(
            string key,
            TimeSpan ttl,
            Func<Task<T>> factory);

        /// <summary>
        /// Gets or creates a cached value using feature name for TTL lookup from configuration
        /// </summary>
        Task<T?> GetOrCreateAsync<T>(
            string featureName,
            string key,
            Func<Task<T>> factory);

        Task RemoveAsync(string key);

        Task ClearAllAsync();

        /// <summary>
        /// Check if a feature has caching enabled
        /// </summary>
        bool IsFeatureEnabled(string featureName);
    }
}
