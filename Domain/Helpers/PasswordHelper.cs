using Domain.Entities;
using Domain.Exceptions;
using System.Security.Cryptography;
using System.Text;

namespace Domain.Helpers;

public static class PasswordHelper
{
	public static (byte[], byte[]) GeneratePasswordHashAndSalt(string password)
	{
		using HMACSHA256 hmac = new();
		var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

		return (passwordHash, hmac.Key);
	}

	public static void VerifyPasswordHash(string password, Player player)
	{
		using HMACSHA256 hmac = new(player.PasswordSalt);
		var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

		if (!computedHash.SequenceEqual(player.PasswordHash))
		{
			throw new IncorrectPasswordException("Incorrect password");
		}
	}
}
