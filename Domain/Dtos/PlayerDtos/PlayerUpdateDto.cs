using System.ComponentModel.DataAnnotations;

namespace Domain.Dtos.PlayerDtos;

public record PlayerUpdateDto([StringLength(30, MinimumLength = 2)] string Name);