namespace Core.Interfaces.Services;

public interface IPasswordService
{
    (byte[], byte[]) GeneratePasswordHashAndSalt(string password);
    void VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
}
