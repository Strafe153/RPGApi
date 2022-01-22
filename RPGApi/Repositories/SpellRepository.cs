using Microsoft.EntityFrameworkCore;
using RPGApi.Data;

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
            throw new NotImplementedException();
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
