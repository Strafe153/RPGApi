using Microsoft.EntityFrameworkCore;
using RPGApi.Data;
using RPGApi.Models;

namespace RPGApi.Repositories
{
    public class WeaponRepository : IControllerRepository<Weapon>
    {
        private readonly DataContext _context;

        public WeaponRepository(DataContext context)
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

        public async Task<IEnumerable<Weapon>> GetAllAsync()
        {
            return await _context.Weapons.ToListAsync();
        }

        public async Task<Weapon?> GetByIdAsync(Guid id)
        {
            return await _context.Weapons.SingleOrDefaultAsync(w => w.Id == id);
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
