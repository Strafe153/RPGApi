using Domain.Enums;

namespace Domain.Dtos.PlayerDtos;

public record PlayerChangeRoleDto
{
    public PlayerRole Role { get; init; } = PlayerRole.Player;
}
