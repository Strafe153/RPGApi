using Microsoft.EntityFrameworkCore;
using RPGApi.Data;

namespace RPGApi.Repositories
{
    public class MountRepository : IControllerRepository<Mount>
    {
        private readonly DataContext _context;

        public MountRepository(DataContext context)
        {
            _context = context;
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
