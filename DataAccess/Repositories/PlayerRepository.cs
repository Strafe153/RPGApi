using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Models;
using DataAccess.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class PlayerRepository : IPlayerRepository
{
    private readonly RPGContext _context;

    public PlayerRepository(RPGContext context)
    {
        _context = context;
    }

    public void Add(Player entity)
    {
        _context.Players.Add(entity);
    }

    public void Delete(Player entity)
    {
        _context.Players.Remove(entity);
    }

    public async Task<PaginatedList<Player>> GetAllAsync(int pageNumber, int pageSize)
    {
        var players = await _context.Players
            .Include(p => p.Characters)
            .ToPaginatedListAsync(pageNumber, pageSize);

        return players;
    }

    public async Task<Player?> GetByIdAsync(int id)
    {
        var player = await _context.Players
            .Include(p => p.Characters)
            .SingleOrDefaultAsync(p => p.Id == id);

        return player;
    }

    public async Task<Player?> GetByNameAsync(string name)
    {
        var player = await _context.Players.SingleOrDefaultAsync(p => p.Name == name);
        return player;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void Update(Player entity)
    {
        _context.Players.Update(entity);
    }
}
