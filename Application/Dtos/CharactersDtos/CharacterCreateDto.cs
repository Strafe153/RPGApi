using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.Dtos.CharactersDtos;

public record CharacterCreateDto(
    [StringLength(30, MinimumLength = 2)] string Name,
    [Required] int PlayerId,
    [Required] CharacterRace Race);
