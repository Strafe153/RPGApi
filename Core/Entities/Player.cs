using Core.Enums;

namespace Core.Entities;

public class Player
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public PlayerRole Role { get; set; }
    public byte[]? PasswordHash { get; set; }
    public byte[]? PasswordSalt { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryDate { get; set; }

    public ICollection<Character> Characters { get; set; } = new List<Character>();
}
