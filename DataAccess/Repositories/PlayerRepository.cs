using Core.Entities;
using Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class PlayerRepository : IRepository<Player>
    {
        private readonly RPGContext _context;

        public PlayerRepository(RPGContext context)
        {
            _context = context;
        }

        public void Create(Player entity)
        {
            _context.Players.Add(entity);
        }

        public void Delete(Player entity)
        {
            _context.Players.Remove(entity);
        }

        public async Task<IEnumerable<Player>> GetAllAsync()
        {
            var players = await _context.Players.ToListAsync();
            return players;
        }

        public async Task<Player?> GetByIdAsync(int id)
        {
            var player = await _context.Players.SingleOrDefaultAsync(p => p.Id == id);
            return player;
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
