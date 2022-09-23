using Core.Enums;

namespace Core.Dtos.CharacterDtos;

public record CharacterBaseDto
{
    public string? Name { get; init; }
    public CharacterRace Race { get; init; }
}
