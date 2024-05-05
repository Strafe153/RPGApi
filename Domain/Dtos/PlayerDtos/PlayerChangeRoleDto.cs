using Domain.Enums;

namespace Domain.Dtos.PlayerDtos;

public record PlayerChangeRoleDto(PlayerRole Role = PlayerRole.Player);