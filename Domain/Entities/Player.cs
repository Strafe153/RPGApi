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

	public ICollection<Character> Characters { get; set; } = new List<Character>();
}
