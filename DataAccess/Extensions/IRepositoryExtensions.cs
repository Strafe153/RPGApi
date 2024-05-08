using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace DataAccess.Extensions;

public static class IRepositoryExtensions
{
	public static async Task<T> GetByIdOrThrowAsync<T>(
		this IRepository<T> repository,
		int id,
		ILogger logger,
		CancellationToken token) where T : class
	{
		var item = await repository.GetByIdAsync(id, token);

		if (item is null)
		{
			logger.LogWarning("Failed to retrieve a weapon with id {Id}", id);
			throw new NullReferenceException($"{typeof(T).Name} with id {id} not found");
		}

		return item;
	}
}
