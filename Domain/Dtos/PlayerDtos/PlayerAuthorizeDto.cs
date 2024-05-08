using System.ComponentModel.DataAnnotations;

namespace Domain.Dtos.PlayerDtos;

public record PlayerAuthorizeDto(
	[StringLength(30, MinimumLength = 2)] string Name,
	[StringLength(50, MinimumLength = 6)] string Password);