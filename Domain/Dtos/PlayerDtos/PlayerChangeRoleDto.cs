using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Dtos.PlayerDtos;

public record PlayerChangeRoleDto([Required] PlayerRole Role);