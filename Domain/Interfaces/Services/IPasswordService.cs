using Domain.Entities;

namespace Domain.Interfaces.Services;

public interface IPasswordService
{
    (byte[], byte[]) GeneratePasswordHashAndSalt(string password);
    void VerifyPasswordHash(string password, Player player);
}
