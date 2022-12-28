using AspNetCoreRateLimit;

namespace API.Extensions;

public static class RateLimitingExtensions
{
    public static void ConfigureRateLmitOptions(this IServiceCollection services, IConfiguration configuration)
    {
        var rateLimitRules = new List<RateLimitRule>
        {
            new RateLimitRule
            {
                Endpoint = "*",
                Limit = configuration.GetValue<int>("RateLimiting:Limit"),
                Period = configuration.GetValue<string>("RateLimiting:Period")
            }
        };

        services.Configure<IpRateLimitOptions>(opt =>
        {
            opt.GeneralRules = rateLimitRules;
        });

        services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
        services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

    }
}