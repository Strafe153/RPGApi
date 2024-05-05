using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Dtos.SpellDtos;

public record SpellUpdateDto(
	[StringLength(30, MinimumLength = 2)]
	string Name,
	SpellType Type = SpellType.Fire,
	[Range(0, 100)]
	int Damage = 12);