using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net.Mime;
using System.Text;

namespace WebApi.Filters;

[AttributeUsage(AttributeTargets.Method)]
public class CacheFilterAttribute(int absoluteExpirationRelativeToNow = 3) : Attribute, IAsyncResourceFilter
{
	public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
	{
		var cacheHelper = context.HttpContext.RequestServices.GetRequiredService<ICacheManager>();

		if (cacheHelper is null)
		{
			await next();
			return;
		}

		var key = GenerateCacheKey(context.HttpContext.Request);
		var cachedResponse = await cacheHelper.GetAsync(key);

		if (!string.IsNullOrEmpty(cachedResponse))
		{
			ContentResult contentResult = new()
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
			var expiration = TimeSpan.FromSeconds(absoluteExpirationRelativeToNow);
			await cacheHelper.SetAsync(key, okObjectResult.Value, expiration);
		}
	}

	private static string GenerateCacheKey(HttpRequest request)
	{
		StringBuilder cacheKeyBuilder = new(request.Path);

		foreach (var (key, value) in request.Query.OrderBy(kv => kv.Key))
		{
			cacheKeyBuilder.Append($"|{key}:{value}");
		}

		return cacheKeyBuilder.ToString();
	}
}
