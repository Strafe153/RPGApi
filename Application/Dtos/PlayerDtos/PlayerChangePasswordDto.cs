using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.PlayerDtos;

public record PlayerChangePasswordDto(
    [StringLength(50, MinimumLength = 6)] string Password);
