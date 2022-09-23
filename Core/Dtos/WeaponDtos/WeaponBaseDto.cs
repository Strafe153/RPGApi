using Core.Enums;

namespace Core.Dtos.WeaponDtos;

public record WeaponBaseDto
{
    public string? Name { get; init; }
    public WeaponType Type { get; init; }
    public int Damage { get; init; } = 15;
}
