using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Dtos.CharacterDtos;

public record CharacterUpdateDto(
	[StringLength(30, MinimumLength = 2)] string Name,
	[Required] CharacterRace Race);