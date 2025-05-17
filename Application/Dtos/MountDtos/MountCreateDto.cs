using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.Dtos.MountDtos;

public record MountCreateDto(
    [StringLength(30, MinimumLength = 2)] string Name,
    [Required] MountType Type,
    [Range(0, 100)] int Speed);
