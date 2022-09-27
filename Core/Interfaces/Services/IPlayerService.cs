using Core.Entities;

namespace Core.Interfaces.Services;

public interface IPlayerService : IService<Player>
{
    Task<Player> GetByNameAsync(string name, CancellationToken token = default);
    Player CreatePlayer(string name, byte[] passwordHash, byte[] passwordSalt);
    void ChangePasswordData(Player player, byte[] passwordHash, byte[] passwordSalt);
    void VerifyPlayerAccessRights(Player performedOn);
}
