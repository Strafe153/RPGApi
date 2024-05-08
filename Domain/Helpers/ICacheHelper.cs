using Microsoft.Extensions.Caching.Distributed;

namespace Domain.Helpers;

public interface ICacheHelper
{
	Task<string?> GetAsync(string key);
	Task SetAsync(string key, object data, DistributedCacheEntryOptions cacheOptions);
}
