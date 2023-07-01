﻿using Core.Shared;

namespace DataAccess.Extensions;

public static class IEnumerableExtensions
{
    public static PaginatedList<T> ToPaginatedList<T>(this IEnumerable<T> enumerable, int pageNumber, int pageSize)
    {
        var count = enumerable.Count();
        var items = enumerable.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }
}
