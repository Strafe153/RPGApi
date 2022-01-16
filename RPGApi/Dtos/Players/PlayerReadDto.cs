using RPGApi.Data;

namespace RPGApi.Dtos.Players
{
    public record PlayerReadDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public PlayerRole Role { get; init; }

        public ICollection<Character> Characters { get; init; }
    }
}
