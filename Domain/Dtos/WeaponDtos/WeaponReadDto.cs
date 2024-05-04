using Domain.Entities;

namespace Domain.Dtos.WeaponDtos;

public record WeaponReadDto : WeaponBaseDto
{
    public int Id { get; init; }
    public IEnumerable<Character>? Characters { get; init; }
}
