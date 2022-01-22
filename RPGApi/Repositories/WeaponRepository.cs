using Microsoft.EntityFrameworkCore;
using RPGApi.Data;

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
            _context.Weapons!.Add(entity);
        }

        public void Delete(Weapon entity)
        {
            _context.Weapons!.Remove(entity);
        }

        public async Task<IEnumerable<Weapon>> GetAllAsync()
        {
            return await _context.Weapons!
                .Include(w => w.Characters)
                .ToListAsync();
        }

        public async Task<Weapon?> GetByIdAsync(Guid id)
        {
            return await _context.Weapons!
                .Include(w => w.Characters)
                .SingleOrDefaultAsync(w => w.Id == id);
        }

        public void LogInformation(string message)
        {
            throw new NotImplementedException();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Update(Weapon entity)
        {
            _context.Weapons!.Update(entity);
        }
    }
}
