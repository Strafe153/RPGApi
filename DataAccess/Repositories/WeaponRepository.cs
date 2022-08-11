using Core.Entities;
using Core.Interfaces.Repositories;
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

        public void Create(Weapon entity)
        {
            _context.Weapons.Add(entity);
        }

        public void Delete(Weapon entity)
        {
            _context.Weapons.Remove(entity);
        }

        public async Task<IEnumerable<Weapon>> GetAllAsync()
        {
            var weapons = await _context.Weapons.ToListAsync();
            return weapons;
        }

        public async Task<Weapon?> GetByIdAsync(int id)
        {
            var weapon = await _context.Weapons.SingleOrDefaultAsync(w => w.Id == id);
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
