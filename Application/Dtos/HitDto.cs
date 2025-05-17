using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.Dtos;

public record HitDto(
    [Required] int DealerId,
    [Required] int ReceiverId,
    [Required] int ItemId,
    [Required] HitType Type);