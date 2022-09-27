﻿using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Models;
using DataAccess.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class MountRepository : IRepository<Mount>
{
    private readonly RPGContext _context;

    public MountRepository(RPGContext context)
    {
        _context = context;
    }

    public void Add(Mount entity)
    {
        _context.Mounts.Add(entity);
    }

    public void Delete(Mount entity)
    {
        _context.Mounts.Remove(entity);
    }

    public async Task<PaginatedList<Mount>> GetAllAsync(int pageNumber, int pageSize, CancellationToken token = default)
    {
        var mounts = await _context.Mounts
            .Include(m => m.CharacterMounts)
                .ThenInclude(cm => cm.Character)
            .OrderBy(c => c.Id)
            .ToPaginatedListAsync(pageNumber, pageSize, token);

        return mounts;
    }

    public async Task<Mount?> GetByIdAsync(int id, CancellationToken token = default)
    {
        var mount = await _context.Mounts
            .Include(m => m.CharacterMounts)
                .ThenInclude(cm => cm.Character)
            .SingleOrDefaultAsync(m => m.Id == id, token);

        return mount;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void Update(Mount entity)
    {
        _context.Mounts.Update(entity);
    }
}
