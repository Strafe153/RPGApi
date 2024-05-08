using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Dtos.WeaponDtos;

public record WeaponUpdateDto(
	[StringLength(30, MinimumLength = 2)] string Name,
	[Required] WeaponType Type,
	[Range(0, 100)] int Damage);