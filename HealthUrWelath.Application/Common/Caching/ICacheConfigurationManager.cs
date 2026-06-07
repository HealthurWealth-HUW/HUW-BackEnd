namespace HealthUrWelath.Application.Common.Caching;

public interface ICacheConfigurationManager
{
    bool IsCacheEnabled();
    bool IsFeatureEnabled(string featureName);
    TimeSpan GetTtl(string featureName);
    string GetCacheStrategy();
}
