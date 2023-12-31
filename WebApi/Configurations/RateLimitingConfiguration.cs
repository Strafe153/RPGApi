using Core.Constants;
using Microsoft.AspNetCore.RateLimiting;
using WebApi.Configurations.ConfigurationModels;

namespace WebApi.Configurations;

public static class RateLimitingConfiguration
{
    public static void ConfigureRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        var rateLimitOptions = configuration.GetSection(RateLimitOptions.SectionName).Get<RateLimitOptions>()!;

        services.AddRateLimiter(options =>
        {
            options.AddTokenBucketLimiter(RateLimitingConstants.TokenBucket, tokenOptions =>
            {
                tokenOptions.TokenLimit = rateLimitOptions.TokenLimit;
                tokenOptions.QueueProcessingOrder = rateLimitOptions.QueueProcessingOrder;
                tokenOptions.QueueLimit = rateLimitOptions.QueueLimit;
                tokenOptions.ReplenishmentPeriod = TimeSpan.FromSeconds(rateLimitOptions.ReplenishmentPeriod);
                tokenOptions.TokensPerPeriod = rateLimitOptions.TokensPerPeriod;
                tokenOptions.AutoReplenishment = rateLimitOptions.AutoReplenishment;
            });

            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.OnRejected = (context, _) =>
            {
                context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.");
                return new ValueTask();
            };
        });
    }
}
