using Microsoft.EntityFrameworkCore;
using RPGApi.Data;
using RPGApi.Repositories.Interfaces;

namespace RPGApi.Repositories.Implementations
{
    public class SpellRepository : IControllerRepository<Spell>
    {
        private readonly DataContext _context;
        private readonly ILogger _logger;

        public SpellRepository(DataContext context, ILogger<SpellRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Add(Spell entity)
        {
            _context.Spells!.Add(entity);
        }

        public void Delete(Spell entity)
        {
            _context.Spells!.Remove(entity);
        }

        public async Task<IEnumerable<Spell>> GetAllAsync()
        {
            return await _context.Spells!
                .Include(s => s.Characters)
                .ToListAsync();
        }

        public async Task<Spell?> GetByIdAsync(Guid id)
        {
            return await _context.Spells!
                .Include(s => s.Characters)
                .SingleOrDefaultAsync(s => s.Id == id);
        }

        public void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Update(Spell entity)
        {
            _context.Spells!.Update(entity);
        }
    }
}
