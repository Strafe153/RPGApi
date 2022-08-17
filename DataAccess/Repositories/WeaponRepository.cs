using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Models;
using DataAccess.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
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

        public async Task<PagedList<Weapon>> GetAllAsync(int pageNumber, int pageSize)
        {
            var weapons = await _context.Weapons
                .Include(w => w.CharacterWeapons)
                    .ThenInclude(cw => cw.Character)
                .ToPagedListAsync(pageNumber, pageSize);

            return weapons;
        }

        public async Task<Weapon?> GetByIdAsync(int id)
        {
            var weapon = await _context.Weapons
                .Include(w => w.CharacterWeapons)
                    .ThenInclude(cw => cw.Character)
                .SingleOrDefaultAsync(w => w.Id == id);

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
}
