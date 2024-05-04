using Domain.Enums;

namespace Domain.Dtos.CharacterDtos;

public record CharacterBaseDto
{
    public string? Name { get; init; }
    public CharacterRace Race { get; init; }
}
