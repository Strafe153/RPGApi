using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;

namespace WebApi.Filters;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class CacheFilterAttribute : Attribute, IAsyncResourceFilter
{
    private readonly int _absoluteExpirationRelativeToNow;
    private readonly int _slidingExpiration;

    public CacheFilterAttribute(int absoluteExpirationRelativeToNow = 60, int slidingExpiration = 10)
    {
        _absoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
        _slidingExpiration = slidingExpiration;
    }

    public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
    {
        var cacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();

        if (cacheService is null)
        {
            await next();
            return;
        }

        var key = GenerateCacheKey(context.HttpContext.Request);
        var cachedResponse = await cacheService.GetAsync(key);

        if (!string.IsNullOrEmpty(cachedResponse))
        {
            var contentResult = new ContentResult
            {
                Content = cachedResponse,
                ContentType = "application/json",
                StatusCode = StatusCodes.Status200OK
            };

            context.Result = contentResult;
            return;
        }

        var executedContext = await next();

        if (executedContext.Result is OkObjectResult okObjectResult)
        {
            await cacheService.SetAsync(key, okObjectResult.Value!, new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_absoluteExpirationRelativeToNow),
                SlidingExpiration = TimeSpan.FromSeconds(_slidingExpiration)
            });
        }
    }

    private static string GenerateCacheKey(HttpRequest request)
    {
        var cacheKeyBuilder = new StringBuilder();

        cacheKeyBuilder.Append(request.Path);

        foreach (var (key, value) in request.Query.OrderBy(kv => kv.Key))
        {
            cacheKeyBuilder.Append($"|{key}:{value}");
        }

        return cacheKeyBuilder.ToString();
    }
}
