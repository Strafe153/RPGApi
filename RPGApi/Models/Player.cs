using RPGApi.Data;

namespace RPGApi.Models
{
    public class Player
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public PlayerRole Role { get; set; }
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }

        public ICollection<Character>? Characters { get; set; }
    }
}
