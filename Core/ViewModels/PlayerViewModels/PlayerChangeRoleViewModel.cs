using Core.Enums;

namespace Core.ViewModels.PlayerViewModels
{
    public record PlayerChangeRoleViewModel
    {
        public PlayerRole Role { get; init; } = PlayerRole.Player;
    }
}
