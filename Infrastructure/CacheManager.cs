using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Text.Json;
using Application.Interfaces;

namespace Infrastructure;

public class CacheManager : ICacheManager
{
	private readonly IDistributedCache _cache;
	private readonly ILogger<CacheManager> _logger;
	private readonly JsonSerializerOptions _serializerOptions;

	public CacheManager(
		IDistributedCache cache,
		ILogger<CacheManager> logger)
	{
		_cache = cache;
		_logger = logger;

		_serializerOptions = new JsonSerializerOptions
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

	public async Task SetAsync<T>(string key, T data, TimeSpan expiration)
	{
		var serializedData = JsonSerializer.Serialize(data, _serializerOptions);
		
		await _cache.SetStringAsync(
			key,
			serializedData,
			new DistributedCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = expiration,
			});

		_logger.LogInformation("Successfully cached data by the key '{Key}'", key);
	}
}
