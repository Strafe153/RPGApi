using Core.Entities;

namespace Core.Interfaces.Services;

public interface IPasswordService
{
    (byte[], byte[]) CreatePasswordHash(string password);
    void VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
    string CreateToken(Player player);
}
