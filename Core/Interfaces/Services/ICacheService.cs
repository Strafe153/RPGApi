using Microsoft.Extensions.Caching.Distributed;

namespace Core.Interfaces.Services;

public interface ICacheService
{
    Task<string> GetAsync(string key);
    Task SetAsync(string key, object data, DistributedCacheEntryOptions cacheOptions);
}
