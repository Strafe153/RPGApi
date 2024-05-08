using Domain.Shared;

namespace Domain.Extensions;

public static class IEnumerableExtensions
{
	public static PagedList<T> ToPagedList<T>(this IEnumerable<T> enumerable, PageParameters pageParameters) => new(
		enumerable,
		enumerable.Count(),
		pageParameters.PageNumber,
		pageParameters.PageSize);
}
