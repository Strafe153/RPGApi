using Microsoft.EntityFrameworkCore;
using RPGApi.Data;
using RPGApi.Repositories.Interfaces;

namespace RPGApi.Repositories.Implementations
{
    public class WeaponRepository : IControllerRepository<Weapon>
    {
        private readonly DataContext _context;
        private readonly ILogger _logger;

        public WeaponRepository(DataContext context, ILogger<WeaponRepository> logger)
        {
            _context = context;
            _logger = logger;
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
            _logger.LogInformation(message);
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
