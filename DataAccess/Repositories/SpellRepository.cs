using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Models;
using DataAccess.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class SpellRepository : IRepository<Spell>
{
    private readonly RPGContext _context;

    public SpellRepository(RPGContext context)
    {
        _context = context;
    }

    public void Add(Spell entity)
    {
        _context.Spells.Add(entity);
    }

    public void Delete(Spell entity)
    {
        _context.Spells.Remove(entity);
    }

    public async Task<PaginatedList<Spell>> GetAllAsync(int pageNumber, int pageSize, CancellationToken token = default)
    {
        var spells = await _context.Spells
            .Include(s => s.CharacterSpells)
                .ThenInclude(cs => cs.Character)
            .OrderBy(c => c.Id)
            .AsNoTracking()
            .ToPaginatedListAsync(pageNumber, pageSize, token);

        return spells;
    }

    public async Task<Spell?> GetByIdAsync(int id, CancellationToken token = default)
    {
        var spell = await _context.Spells
            .Include(s => s.CharacterSpells)
                .ThenInclude(cs => cs.Character)
            .FirstOrDefaultAsync(s => s.Id == id, token);

        return spell;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void Update(Spell entity)
    {
        _context.Spells.Update(entity);
    }
}
