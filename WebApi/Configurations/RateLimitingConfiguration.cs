using Core.Constants;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace WebApi.Configurations;

public static class RateLimitingConfiguration
{
    public static void ConfigureRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRateLimiter(options =>
        {
            var rateLimitOptions = configuration
                .GetSection(RateLimitingConstants.SectionName)
                .Get<TokenBucketRateLimiterOptions>()!;

            options.AddTokenBucketLimiter(RateLimitingConstants.TokenBucket, options => options = rateLimitOptions);
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.OnRejected = (context, _) =>
            {
                context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.");
                return new ValueTask();
            };
        });
    }
}
