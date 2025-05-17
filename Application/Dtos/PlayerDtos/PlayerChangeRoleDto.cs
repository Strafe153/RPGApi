using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.Dtos.PlayerDtos;

public record PlayerChangeRoleDto(
    [Required] PlayerRole Role);