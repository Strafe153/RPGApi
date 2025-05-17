using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.Dtos.SpellDtos;

public record SpellCreateDto(
    [StringLength(30, MinimumLength = 2)] string Name,
    [Required] SpellType Type,
    [Range(0, 100)] int Damage);