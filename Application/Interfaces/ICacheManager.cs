namespace Application.Interfaces;

public interface ICacheManager
{
	Task<string?> GetAsync(string key);
	Task SetAsync<T>(string key, T data, TimeSpan expiration);
}
