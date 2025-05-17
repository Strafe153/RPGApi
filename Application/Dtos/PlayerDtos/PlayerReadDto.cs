using Domain.Entities;
using Domain.Enums;

namespace Application.Dtos.PlayerDtos;

public record PlayerReadDto(
    int Id,
    string Name,
    PlayerRole Role,
    IEnumerable<Character> Characters);
