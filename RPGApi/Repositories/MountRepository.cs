using Microsoft.EntityFrameworkCore;
using RPGApi.Data;

namespace RPGApi.Repositories
{
    public class MountRepository : IControllerRepository<Mount>
    {
        private readonly DataContext _context;
        private readonly ILogger _logger;

        public MountRepository(DataContext context, ILogger<MountRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Add(Mount entity)
        {
            _context.Mounts!.Add(entity);
        }

        public void Delete(Mount entity)
        {
            _context.Mounts!.Remove(entity);
        }

        public async Task<IEnumerable<Mount>> GetAllAsync()
        {
            return await _context.Mounts!
                .Include(m => m.Characters)
                .ToListAsync();
        }

        public async Task<Mount?> GetByIdAsync(Guid id)
        {
            return await _context.Mounts!
                .Include(m => m.Characters)
                .SingleOrDefaultAsync(m => m.Id == id);
        }

        public void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Update(Mount entity)
        {
            _context.Mounts!.Update(entity);
        }
    }
}
