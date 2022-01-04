using Microsoft.EntityFrameworkCore;
using RPGApi.Data;
using RPGApi.Models;

namespace RPGApi.Repositories
{
    public class SpellRepository : IControllerRepository<Spell>
    {
        private readonly DataContext _context;

        public SpellRepository(DataContext context)
        {
            _context = context;
        }

        public void Add(Spell entity)
        {
            _context.Spells.Add(entity);
        }

        public void Delete(Spell entity)
        {
            _context.Spells.Remove(entity);
        }

        public async Task<IEnumerable<Spell>> GetAllAsync()
        {
            return await _context.Spells.ToListAsync();
        }

        public async Task<Spell?> GetByIdAsync(Guid id)
        {
            return await _context.Spells.SingleOrDefaultAsync(s => s.Id == id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Update(Spell entity)
        {
            _context.Spells.Update(entity);
        }
    }
}
