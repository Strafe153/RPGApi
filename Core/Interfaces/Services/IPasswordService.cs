using Core.Entities;

namespace Core.Interfaces.Services;

public interface IPasswordService
{
    void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
    void VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
    string CreateToken(Player player);
}
