using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Dtos.CharacterDtos;

public record CharacterCreateDto(
	[StringLength(30, MinimumLength = 2)] string Name,
	[Required] int PlayerId,
	[Required] CharacterRace Race);