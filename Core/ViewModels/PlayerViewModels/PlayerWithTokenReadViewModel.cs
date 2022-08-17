using Core.Enums;

namespace Core.ViewModels.PlayerViewModels
{
    public record PlayerWithTokenReadViewModel
    {
        public int Id { get; init; }
        public string? Name { get; init; }
        public PlayerRole Role { get; init; }
        public string? Token { get; init; }
    }
}
