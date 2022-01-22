using Microsoft.EntityFrameworkCore;
using RPGApi.Data;

namespace RPGApi.Repositories
{
    public class CharacterRepository : IControllerRepository<Character>
    {
        private readonly DataContext _context;
        private readonly ILogger _logger;

        public CharacterRepository(DataContext context, ILogger<CharacterRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Add(Character entity)
        {
            _context.Characters!.Add(entity);
        }

        public void Delete(Character entity)
        {
            _context.Characters!.Remove(entity);
        }

        public async Task<IEnumerable<Character>> GetAllAsync()
        {
            return await _context.Characters!
                .Include(c => c.Weapons)
                .Include(c => c.Spells)
                .Include(c => c.Mounts)
                .ToListAsync();
        }

        public async Task<Character?> GetByIdAsync(Guid id)
        {
            return await _context.Characters!
                .Include(c => c.Weapons)
                .Include(c => c.Spells)
                .Include(c => c.Mounts)
                .SingleOrDefaultAsync(c => c.Id == id);
        }

        public void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Update(Character entity)
        {
            _context.Characters!.Update(entity);
        }
    }
}
