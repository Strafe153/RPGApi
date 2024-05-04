using Domain.Enums;

namespace Domain.Dtos.WeaponDtos;

public record WeaponBaseDto
{
    public string? Name { get; init; }
    public WeaponType Type { get; init; }
    public int Damage { get; init; } = 15;
}
