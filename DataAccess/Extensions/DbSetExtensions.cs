﻿using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Extensions
{
    public static class DbSetExtensions
    {
        public static async Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> query, int pageNumber, int pageSize)
        {
            int count = query.Count();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}