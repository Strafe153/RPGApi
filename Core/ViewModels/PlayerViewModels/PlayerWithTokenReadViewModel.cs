using Core.Enums;

namespace Core.ViewModels.PlayerViewModels
{
    public record PlayerWithTokenReadViewModel : PlayerBaseViewModel
    {
        public int Id { get; init; }
        public PlayerRole Role { get; init; }
        public string? Token { get; init; }
    }
}
