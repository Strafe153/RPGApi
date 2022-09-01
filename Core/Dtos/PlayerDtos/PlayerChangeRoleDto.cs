using Core.Enums;

namespace Core.Dtos.PlayerDtos
{
    public record PlayerChangeRoleDto
    {
        public PlayerRole Role { get; init; } = PlayerRole.Player;
    }
}
