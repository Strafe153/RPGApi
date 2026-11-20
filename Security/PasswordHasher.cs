using System.Security.Cryptography;
using System.Text;
using Security.Exceptions;

namespace Security;

public static class PasswordHasher
{
	public static (byte[], byte[]) GenerateHashWithSalt(string password)
	{
		using HMACSHA256 hmac = new();
		var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

		return (passwordHash, hmac.Key);
	}

	public static void Verify(string password, byte[] salt, byte[] expectedHash)
	{
		using HMACSHA256 hmac = new(salt);
		var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

		if (!computedHash.SequenceEqual(expectedHash))
		{
			throw new IncorrectPasswordException("Incorrect password");
		}
	}
}