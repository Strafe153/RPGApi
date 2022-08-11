using Core.Entities;
using Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class CharacterRepository : IRepository<Character>
    {
        private readonly RPGContext _context;

        public CharacterRepository(RPGContext context)
        {
            _context = context;
        }

        public void Create(Character entity)
        {
            _context.Characters.Add(entity);
        }

        public void Delete(Character entity)
        {
            _context.Characters.Remove(entity);
        }

        public async Task<IEnumerable<Character>> GetAllAsync()
        {
            var characters = await _context.Characters.ToListAsync();
            return characters;
        }

        public async Task<Character?> GetByIdAsync(int id)
        {
            var character = await _context.Characters.SingleOrDefaultAsync(c => c.Id == id);
            return character;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Update(Character entity)
        {
            _context.Characters.Update(entity);
        }
    }
}
