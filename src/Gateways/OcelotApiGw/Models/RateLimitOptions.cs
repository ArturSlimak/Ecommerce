namespace OcelotApiGw.Models;

public class RateLimitOptions
{
    public const string RateLimit = "RateLimit";
    public TokenBucketOptions TokenBucket { get; set; } = new();
}

public class TokenBucketOptions
{
    public string SectionName => "TokenBucket";
    public string PolicyName { get; set; } = null!;
    public int TokenLimit { get; set; }
    public int QueueLimit { get; set; }
    public int ReplenishmentPeriod { get; set; }
    public int TokensPerPeriod { get; set; }
    public bool AutoReplenishment { get; set; }
}

