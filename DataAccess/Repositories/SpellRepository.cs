using Core.Entities;
using Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class SpellRepository : IRepository<Spell>
    {
        private readonly RPGContext _context;

        public SpellRepository(RPGContext context)
        {
            _context = context;
        }

        public void Create(Spell entity)
        {
            _context.Spells.Add(entity);
        }

        public void Delete(Spell entity)
        {
            _context.Spells.Remove(entity);
        }

        public async Task<IEnumerable<Spell>> GetAllAsync()
        {
            var spells = await _context.Spells.ToListAsync();
            return spells;
        }

        public async Task<Spell?> GetByIdAsync(int id)
        {
            var spell = await _context.Spells.SingleOrDefaultAsync(s => s.Id == id);
            return spell;
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
