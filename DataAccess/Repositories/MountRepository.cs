using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Models;
using DataAccess.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class MountRepository : IRepository<Mount>
    {
        private readonly RPGContext _context;

        public MountRepository(RPGContext context)
        {
            _context = context;
        }

        public void Add(Mount entity)
        {
            _context.Mounts.Add(entity);
        }

        public void Delete(Mount entity)
        {
            _context.Mounts.Remove(entity);
        }

        public async Task<PagedList<Mount>> GetAllAsync(int pageNumber, int pageSize)
        {
            var mounts = await _context.Mounts
                .Include(m => m.CharacterMounts)
                    .ThenInclude(cm => cm.Character)
                .ToPagedListAsync(pageNumber, pageSize);

            return mounts;
        }

        public async Task<Mount?> GetByIdAsync(int id)
        {
            var mount = await _context.Mounts
                .Include(m => m.CharacterMounts)
                    .ThenInclude(cm => cm.Character)
                .SingleOrDefaultAsync(m => m.Id == id);

            return mount;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Update(Mount entity)
        {
            _context.Mounts.Update(entity);
        }
    }
}
