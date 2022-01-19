using RPGApi.Data;

namespace RPGApi.Dtos.Players
{
    public record PlayerWithTokenReadDto
    {
        public Guid Id { get; init; }
        public string? Name { get; init; }
        public PlayerRole Role { get; init; }
        public string? Token { get; init; }
    }
}
