using Domain.Entities;
using Domain.Enums;

namespace Domain.Dtos.PlayerDtos;

public record PlayerReadDto : PlayerBaseDto
{
    public int Id { get; init; }
    public PlayerRole Role { get; init; }
    public IEnumerable<Character>? Characters { get; init; }
}
