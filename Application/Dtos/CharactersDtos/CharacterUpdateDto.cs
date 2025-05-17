using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.Dtos.CharactersDtos;

public record CharacterUpdateDto(
    [StringLength(30, MinimumLength = 2)] string Name,
    [Required] CharacterRace Race);