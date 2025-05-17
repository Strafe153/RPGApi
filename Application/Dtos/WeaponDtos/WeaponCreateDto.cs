using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.Dtos.WeaponDtos;

public record WeaponCreateDto(
    [StringLength(30, MinimumLength = 2)] string Name,
    [Required] WeaponType Type,
    [Range(0, 100)] int Damage);
