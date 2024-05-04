using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces.Services;
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
        var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        var passwordSalt = hmac.Key;

        return (passwordHash, passwordSalt);
    }

    public void VerifyPasswordHash(string password, Player player)
    {
        using var hmac = new HMACSHA256(player.PasswordSalt!);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        if (!computedHash.SequenceEqual(player.PasswordHash!))
        {
            _logger.LogWarning("Player failed to log in due to providing an incorrect password");
            throw new IncorrectPasswordException("Incorrect password");
        }

        _logger.LogInformation("Player successfully logged in");
    }
}
