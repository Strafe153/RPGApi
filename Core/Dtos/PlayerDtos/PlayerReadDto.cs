using Core.Entities;
using Core.Enums;

namespace Core.Dtos.PlayerDtos;

public record PlayerReadDto : PlayerBaseDto
{
    public int Id { get; init; }
    public PlayerRole Role { get; init; }
    public IEnumerable<Character>? Characters { get; init; }
}
