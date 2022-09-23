using Core.Enums;

namespace Core.Dtos.PlayerDtos;

public record PlayerWithTokenReadDto : PlayerBaseDto
{
    public int Id { get; init; }
    public PlayerRole Role { get; init; }
    public string? Token { get; init; }
}
