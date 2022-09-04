using Core.Entities;
using System.Security.Claims;
using System.Security.Principal;

namespace Core.Interfaces.Services
{
    public interface IPlayerService : IService<Player>
    {
        Task<Player> GetByNameAsync(string name);
        Player CreatePlayer(string name, byte[] passwordHash, byte[] passwordSalt);
        void ChangePasswordData(Player player, byte[] passwordHash, byte[] passwordSalt);
        void VerifyPlayerAccessRights(Player performedOn, IIdentity performer, IEnumerable<Claim> claims);
    }
}
