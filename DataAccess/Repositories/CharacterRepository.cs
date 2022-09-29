using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Models;
using DataAccess.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class CharacterRepository : IRepository<Character>
{
    private readonly RPGContext _context;

    public CharacterRepository(RPGContext context)
    {
        _context = context;
    }

    public void Add(Character entity)
    {
        _context.Characters.Add(entity);
    }

    public void Delete(Character entity)
    {
        _context.Characters.Remove(entity);
    }

    public async Task<PaginatedList<Character>> GetAllAsync(int pageNumber, int pageSize, CancellationToken token = default)
    {
        var characters = await _context.Characters
            .Include(c => c.CharacterWeapons)
                .ThenInclude(cw => cw.Weapon)
            .Include(c => c.CharacterSpells)
                .ThenInclude(cs => cs.Spell)
            .Include(c => c.CharacterMounts)
                .ThenInclude(cm => cm.Mount)
            .OrderBy(c => c.Id)
            .ToPaginatedListAsync(pageNumber, pageSize, token);

        return characters;
    }

    public async Task<Character?> GetByIdAsync(int id, CancellationToken token = default)
    {
        var character = await _context.Characters
            .Include(c => c.Player)
            .Include(c => c.CharacterWeapons)
                .ThenInclude(cw => cw.Weapon)
            .Include(c => c.CharacterSpells)
                .ThenInclude(cs => cs.Spell)
            .Include(c => c.CharacterMounts)
                .ThenInclude(cm => cm.Mount)
            .FirstOrDefaultAsync(c => c.Id == id, token);

        return character;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void Update(Character entity)
    {
        _context.Characters.Update(entity);
    }
}
