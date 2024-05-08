using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Dtos.SpellDtos;

public record SpellCreateDto(
	[StringLength(30, MinimumLength = 2)] string Name,
	[Required] SpellType Type,
	[Range(0, 100)] int Damage);