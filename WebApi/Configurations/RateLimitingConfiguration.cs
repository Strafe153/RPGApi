using Core.Constants;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace WebApi.Configurations;

public static class RateLimitingConfiguration
{
    public static void ConfigureRateLimiting(this IServiceCollection services, IConfiguration configuration) =>
        services.AddRateLimiter(options =>
        {
            options.AddTokenBucketLimiter(RateLimitingConstants.TokenBucket, tokenOptions =>
            {
                var rateLimitOptions = configuration
                    .GetSection(RateLimitingConstants.SectionName)
                    .Get<TokenBucketRateLimiterOptions>()!;

                tokenOptions.TokenLimit = rateLimitOptions.TokenLimit;
                tokenOptions.TokensPerPeriod = rateLimitOptions.TokensPerPeriod;
                tokenOptions.ReplenishmentPeriod = rateLimitOptions.ReplenishmentPeriod;
                tokenOptions.AutoReplenishment = rateLimitOptions.AutoReplenishment;
                tokenOptions.QueueLimit = rateLimitOptions.QueueLimit;
                tokenOptions.QueueProcessingOrder = rateLimitOptions.QueueProcessingOrder;

            });

            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.OnRejected = (context, _) =>
            {
                context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.");
                return new ValueTask();
            };
        });
}
