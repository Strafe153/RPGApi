using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Models;
using DataAccess.Extensions;
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

        public void Add(Character entity)
        {
            _context.Characters.Add(entity);
        }

        public void Delete(Character entity)
        {
            _context.Characters.Remove(entity);
        }

        public async Task<PaginatedList<Character>> GetAllAsync(int pageNumber, int pageSize)
        {
            var characters = await _context.Characters
                .Include(c => c.CharacterWeapons)
                    .ThenInclude(cw => cw.Weapon)
                .Include(c => c.CharacterSpells)
                    .ThenInclude(cs => cs.Spell)
                .Include(c => c.CharacterMounts)
                    .ThenInclude(cm => cm.Mount)
                .ToPaginatedListAsync(pageNumber, pageSize);

            return characters;
        }

        public async Task<Character?> GetByIdAsync(int id)
        {
            var character = await _context.Characters
                .Include(c => c.CharacterWeapons)
                    .ThenInclude(cw => cw.Weapon)
                .Include(c => c.CharacterSpells)
                    .ThenInclude(cs => cs.Spell)
                .Include(c => c.CharacterMounts)
                    .ThenInclude(cm => cm.Mount)
                .SingleOrDefaultAsync(c => c.Id == id);

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
