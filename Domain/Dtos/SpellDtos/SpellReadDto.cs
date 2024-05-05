using Domain.Entities;
using Domain.Enums;

namespace Domain.Dtos.SpellDtos;

public record SpellReadDto(
	int Id,
	string Name,
	SpellType Type,
	int Damage,
	IEnumerable<Character> Characters);