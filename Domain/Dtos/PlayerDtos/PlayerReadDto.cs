using Domain.Entities;
using Domain.Enums;

namespace Domain.Dtos.PlayerDtos;

public record PlayerReadDto(
	int Id,
	string Name,
	PlayerRole Role,
	IEnumerable<Character> Characters);