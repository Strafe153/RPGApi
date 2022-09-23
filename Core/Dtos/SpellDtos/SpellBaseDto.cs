using Core.Enums;

namespace Core.Dtos.SpellDtos;

public record SpellBaseDto
{
    public string? Name { get; init; }
    public SpellType Type { get; init; }
    public int Damage { get; init; } = 12;
}
