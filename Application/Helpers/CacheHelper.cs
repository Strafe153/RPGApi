using Domain.Helpers;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Application.Helpers;

public class CacheHelper : ICacheHelper
{
	private readonly IDistributedCache _cache;
	private readonly ILogger<CacheHelper> _logger;
	private readonly JsonSerializerOptions _serializerOptions;

	public CacheHelper(
		IDistributedCache cache,
		ILogger<CacheHelper> logger)
	{
		_cache = cache;
		_logger = logger;

		_serializerOptions = new JsonSerializerOptions()
		{
			ReferenceHandler = ReferenceHandler.IgnoreCycles,
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase
		};
	}

	public async Task<string?> GetAsync(string key)
	{
		var cachedData = await _cache.GetStringAsync(key);
		_logger.LogInformation("Successfully retrieved cached data by the key '{Key}'", key);

		return cachedData;
	}

	public async Task SetAsync(string key, object data, DistributedCacheEntryOptions cacheOptions)
	{
		var serializedData = JsonSerializer.Serialize(data, _serializerOptions);
		await _cache.SetStringAsync(key, serializedData, cacheOptions);

		_logger.LogInformation("Successfully cached data by the key '{Key}'", key);
	}
}
