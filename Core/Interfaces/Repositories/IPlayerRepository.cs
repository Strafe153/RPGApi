using Core.Entities;

namespace Core.Interfaces.Repositories
{
    public interface IPlayerRepository : IRepository<Player>
    {
        Task<Player?> GetByNameAsync(string name);
        void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
        string CreateToken(Player player);
    }
}
