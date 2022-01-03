using Microsoft.EntityFrameworkCore;
using RPGApi.Data;
using RPGApi.Models;

namespace RPGApi.Repositories
{
    public class PlayerRepository : IControllerRepository<Player>
    {
        private readonly DataContext _context;

        public PlayerRepository(DataContext context)
        {
            _context = context;
        }

        public void Add(Player entity)
        {
            _context.Players.Add(entity);
        }

        public void Delete(Player entity)
        {
            _context.Players.Remove(entity);
        }

        public async Task<IEnumerable<Player>> GetAllAsync()
        {
            return await _context.Players
                .Include(p => p.Characters)
                .ToListAsync();
        }

        public async Task<Player?> GetByIdAsync(Guid id)
        {
            return await _context.Players
                .Include(p => p.Characters)
                .SingleOrDefaultAsync(p => p.Id == id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Update(Player entity)
        {
            _context.Players.Update(entity);
        }
    }
}
