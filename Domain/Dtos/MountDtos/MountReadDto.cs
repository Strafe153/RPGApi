using Domain.Entities;
using Domain.Enums;

namespace Domain.Dtos.MountDtos;

public record MountReadDto(
	int Id,
	string Name,
	MountType Type,
	int Speed,
	IEnumerable<Character> Characters);