using Microsoft.Extensions.Configuration;
using HealthUrWelath.Application.Authentication.Interfaces;

namespace HealthUrWealth.Infrastructure.Common
{
    public sealed class AppSettings : IAppSettings
    {
        private readonly IConfiguration _config;

        public AppSettings(IConfiguration config)
        {
            _config = config;
        }

        public string? Get(string key)
        {
            var value = _config[key];
            if (!string.IsNullOrEmpty(value))
                return value;
            // Do not fallback to environment variables here — configuration should come from IConfiguration
            return null;
        }
    }
}
