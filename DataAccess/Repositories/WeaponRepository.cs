using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Models;
using DataAccess.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class WeaponRepository : IRepository<Weapon>
{
    private readonly RPGContext _context;

    public WeaponRepository(RPGContext context)
    {
        _context = context;
    }

    public void Add(Weapon entity)
    {
        _context.Weapons.Add(entity);
    }

    public void Delete(Weapon entity)
    {
        _context.Weapons.Remove(entity);
    }

    public async Task<PaginatedList<Weapon>> GetAllAsync(int pageNumber, int pageSize, CancellationToken token = default)
    {
        var weapons = await _context.Weapons
            .Include(w => w.CharacterWeapons)
                .ThenInclude(cw => cw.Character)
            .OrderBy(c => c.Id)
            .AsNoTracking()
            .ToPaginatedListAsync(pageNumber, pageSize, token);

        return weapons;
    }

    public async Task<Weapon?> GetByIdAsync(int id, CancellationToken token = default)
    {
        var weapon = await _context.Weapons
            .Include(w => w.CharacterWeapons)
                .ThenInclude(cw => cw.Character)
            .FirstOrDefaultAsync(w => w.Id == id, token);

        return weapon;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void Update(Weapon entity)
    {
        _context.Weapons.Update(entity);
    }
}
