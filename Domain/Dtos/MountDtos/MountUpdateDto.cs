using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Dtos.MountDtos;

public record MountUpdateDto(
	[StringLength(30, MinimumLength = 2)]
	string Name,
	MountType Type = MountType.Horse,
	[Range(0, 100)]
	int Speed = 8);