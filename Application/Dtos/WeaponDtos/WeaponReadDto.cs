using Domain.Entities;
using Domain.Enums;

namespace Application.Dtos.WeaponDtos;

public record WeaponReadDto(
    int Id,
    string Name,
    WeaponType Type,
    int Damage,
    IEnumerable<Character> Characters);