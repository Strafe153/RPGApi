using Core.Interfaces.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text;
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

        _serializerOptions = new()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        byte[] cachedData = await _cache.GetAsync(key);

        if (cachedData is not null)
        {
            string serializedData = Encoding.UTF8.GetString(cachedData);
            var result = JsonSerializer.Deserialize<T>(serializedData)!;

            _logger.LogInformation("Successfully retrieved cached data of type '{Type}'", typeof(T));

            return result;
        }

        _logger.LogInformation("Cached data of type '{Type}' does not exist", typeof(T));

        return default;
    }

    public async Task SetAsync<T>(string key, T data)
    {
        string serializedData = JsonSerializer.Serialize(data, _serializerOptions);
        byte[] dataAsBytes = Encoding.UTF8.GetBytes(serializedData);

        await _cache.SetAsync(key, dataAsBytes, new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1),
            SlidingExpiration = TimeSpan.FromSeconds(10)
        });

        _logger.LogInformation("Successfully cached data of type '{Type}'", typeof(T));
    }
}
