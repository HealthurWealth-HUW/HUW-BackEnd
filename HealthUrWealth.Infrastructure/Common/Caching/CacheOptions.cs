namespace HealthUrWealth.Infrastructure.Common.Caching;

public class CacheOptions
{
    public bool Enabled { get; set; } = true;
    public string Strategy { get; set; } = "Memory";
    public int DefaultTtlMinutes { get; set; } = 15;
    public Dictionary<string, CacheFeatureOptions> Features { get; set; } = new();
}

public class CacheFeatureOptions
{
    public bool Enabled { get; set; } = true;
    public int TtlMinutes { get; set; } = 15;
    public string? Description { get; set; }
}
