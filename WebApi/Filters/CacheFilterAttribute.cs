using Application.Services.Abstractions;
using Domain.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using System.Net.Mime;
using System.Text;

namespace WebApi.Filters;

[AttributeUsage(AttributeTargets.Method)]
public class CacheFilterAttribute : Attribute, IAsyncResourceFilter
{
	private readonly int _absoluteExpirationRelativeToNow;

	public CacheFilterAttribute(int absoluteExpirationRelativeToNow = 3)
	{
		_absoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
	}

	public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
	{
		var cacheHelper = context.HttpContext.RequestServices.GetRequiredService<ICacheHelper>();

		if (cacheHelper is null)
		{
			await next();
			return;
		}

		var key = GenerateCacheKey(context.HttpContext.Request);
		var cachedResponse = await cacheHelper.GetAsync(key);

		if (!string.IsNullOrEmpty(cachedResponse))
		{
			var contentResult = new ContentResult
			{
				Content = cachedResponse,
				ContentType = MediaTypeNames.Application.Json,
				StatusCode = StatusCodes.Status200OK
			};

			context.Result = contentResult;
			return;
		}

		var executedContext = await next();

		if (executedContext.Result is OkObjectResult okObjectResult
			&& okObjectResult.Value is not null)
		{
			await cacheHelper.SetAsync(key, okObjectResult.Value, new DistributedCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_absoluteExpirationRelativeToNow)
			});
		}
	}

	private static string GenerateCacheKey(HttpRequest request)
	{
		var cacheKeyBuilder = new StringBuilder(request.Path);

		foreach (var (key, value) in request.Query.OrderBy(kv => kv.Key))
		{
			cacheKeyBuilder.Append($"|{key}:{value}");
		}

		return cacheKeyBuilder.ToString();
	}
}
