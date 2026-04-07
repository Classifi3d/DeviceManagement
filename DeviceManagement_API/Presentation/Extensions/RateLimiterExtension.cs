using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace Presentation.Configurations;

public static class RateLimiterExtension
{
    public static IServiceCollection AddEndpointRateLimiters( this IServiceCollection services )
    {
        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("registerLimiter", opt =>
            {
                opt.PermitLimit = 5;
                opt.Window = TimeSpan.FromMinutes(1);
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = 2;
            });

            options.AddTokenBucketLimiter("loginLimiter", opt =>
            {
                opt.TokenLimit = 10;
                opt.TokensPerPeriod = 2;
                opt.ReplenishmentPeriod = TimeSpan.FromSeconds(10);
                opt.AutoReplenishment = true;
                opt.QueueLimit = 3;
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });
        });

        return services;
    }
}
