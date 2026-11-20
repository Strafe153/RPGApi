using System.Security.Claims;
using Domain.Enums;

namespace Domain.Entities;

public class Player
{
	public int Id { get; set; }
	public string Name { get; set; } = default!;
	public PlayerRole Role { get; set; }
	public byte[] PasswordHash { get; set; } = default!;
	public byte[] PasswordSalt { get; set; } = default!;
	public string? RefreshToken { get; set; }
	public DateTime RefreshTokenExpiryDate { get; set; }

	public ICollection<Character> Characters { get; set; } = [];

	public Claim[] GetClaims()
	{
		Claim[] claims = [
            new(ClaimTypes.Name, Name),
			new(ClaimTypes.Role, Role.ToString()),
			new(nameof(Id), Id.ToString())
		];

		return claims;
	}

	public void SetRefreshToken(string refreshToken, int expirationPeriod)
	{
		RefreshToken = refreshToken;
		RefreshTokenExpiryDate = DateTime.UtcNow.AddDays(expirationPeriod);
	}

	public bool ValidateRefreshToken(string refreshToken)
	{
		if (RefreshToken != refreshToken)
		{
			return false;
		}

		if (RefreshTokenExpiryDate < DateTime.UtcNow)
		{
			return false;
		}

		return true;
	}
}
