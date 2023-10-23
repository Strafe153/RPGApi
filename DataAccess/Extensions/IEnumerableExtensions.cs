using Core.Shared;

namespace DataAccess.Extensions;

public static class IEnumerableExtensions
{
    public static PaginatedList<T> ToPaginatedList<T>(this IEnumerable<T> enumerable, int pageNumber, int pageSize) =>
        new PaginatedList<T>(enumerable, enumerable.Count(), pageNumber, pageSize);
}
