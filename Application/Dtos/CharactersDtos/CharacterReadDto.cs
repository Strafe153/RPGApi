using Domain.Entities;
using Domain.Enums;

namespace Application.Dtos.CharactersDtos;

public record CharacterReadDto(
    int Id,
    string Name,
    CharacterRace Race,
    int Health,
    int PlayerId,
    IEnumerable<Weapon> Weapons,
    IEnumerable<Spell> Spells,
    IEnumerable<Mount> Mounts);