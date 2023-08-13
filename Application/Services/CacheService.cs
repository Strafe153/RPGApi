using Core.Interfaces.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application.Services;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<CacheService> _logger;
    private readonly JsonSerializerOptions _serializerOptions;

    public CacheService(
        IDistributedCache cache,
        ILogger<CacheService> logger)
    {
        _cache = cache;
        _logger = logger;

        _serializerOptions = new JsonSerializerOptions()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<string> GetAsync(string key)
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
