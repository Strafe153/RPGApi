using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Extensions;

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
			var entityType = typeof(T).Name;
			logger.LogWarning("Failed to retrieve an {entityType} with id {Id}", entityType, id);
			
			throw new NullReferenceException($"{entityType} with id {id} not found");
		}

		return item;
	}
}
