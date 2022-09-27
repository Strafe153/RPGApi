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

    public async Task<PaginatedList<Player>> GetAllAsync(int pageNumber, int pageSize, CancellationToken token = default)
    {
        var players = await _context.Players
            .Include(p => p.Characters)
            .ToPaginatedListAsync(pageNumber, pageSize, token);

        return players;
    }

    public async Task<Player?> GetByIdAsync(int id, CancellationToken token = default)
    {
        var player = await _context.Players
            .Include(p => p.Characters)
            .SingleOrDefaultAsync(p => p.Id == id, token);

        return player;
    }

    public async Task<Player?> GetByNameAsync(string name, CancellationToken token = default)
    {
        var player = await _context.Players.SingleOrDefaultAsync(p => p.Name == name, token);
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
