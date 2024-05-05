using Domain.Entities;
using Domain.Enums;

namespace Domain.Dtos.CharacterDtos;

public record CharacterReadDto(
	int Id,
	string Name,
	CharacterRace Race,
	int Health,
	int PlayerId,
	IEnumerable<Weapon> Weapons,
	IEnumerable<Spell> Spells,
	IEnumerable<Mount> Mounts);