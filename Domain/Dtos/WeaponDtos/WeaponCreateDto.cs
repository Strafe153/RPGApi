using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Dtos.WeaponDtos;

public record WeaponCreateDto(
	[StringLength(30, MinimumLength = 2)]
	string Name,
	WeaponType Type = WeaponType.Sword,
	[Range(0, 100)]
	int Damage = 15);