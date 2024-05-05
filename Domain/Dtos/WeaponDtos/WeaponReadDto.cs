using Domain.Entities;
using Domain.Enums;

namespace Domain.Dtos.WeaponDtos;

public record WeaponReadDto(
	int Id,
	string Name,
	WeaponType Type,
	int Damage,
	IEnumerable<Character> Characters);