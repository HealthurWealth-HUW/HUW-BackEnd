using HealthUrWelath.Application.Common.Caching;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HealthUrWealth.Infrastructure.Common.Caching;

public sealed class CacheConfigurationManager : ICacheConfigurationManager
{
    private readonly IOptions<CacheOptions> _options;
    private readonly ILogger<CacheConfigurationManager> _logger;

    public CacheConfigurationManager(
        IOptions<CacheOptions> options,
        ILogger<CacheConfigurationManager> logger)
    {
        _options = options;
        _logger = logger;
    }

    public bool IsCacheEnabled() => _options.Value.Enabled;

    public bool IsFeatureEnabled(string featureName)
    {
        if (!_options.Value.Enabled)
            return false;

        if (!_options.Value.Features.TryGetValue(featureName, out var feature))
        {
            _logger.LogWarning("Cache feature '{FeatureName}' not configured", featureName);
            return false;
        }

        return feature.Enabled;
    }

    public TimeSpan GetTtl(string featureName)
    {
        if (_options.Value.Features.TryGetValue(featureName, out var feature))
            return TimeSpan.FromMinutes(feature.TtlMinutes);

        return TimeSpan.FromMinutes(_options.Value.DefaultTtlMinutes);
    }

    public string GetCacheStrategy() => _options.Value.Strategy;
}
