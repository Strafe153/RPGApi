using Core.Exceptions;
using Core.Interfaces.Services;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services;

public class PasswordService : IPasswordService
{
    private readonly ILogger<PasswordService> _logger;

    public PasswordService(ILogger<PasswordService> logger)
    {
        _logger = logger;
    }

    public (byte[], byte[]) GeneratePasswordHashAndSalt(string password)
    {
        using var hmac = new HMACSHA256();
        byte[] passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        byte[] passwordSalt = hmac.Key;

        return (passwordHash, passwordSalt);
    }

    public void VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using var hmac = new HMACSHA256(passwordSalt);
        byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        if (!computedHash.SequenceEqual(passwordHash))
        {
            _logger.LogWarning("Player failed to log in due to providing an incorrect password");
            throw new IncorrectPasswordException("Incorrect password");
        }

        _logger.LogInformation("Player successfully logged in");
    }
}
