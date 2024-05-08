using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Dtos;

public record HitDto(
	[Required] int DealerId,
	[Required] int ReceiverId,
	[Required] int ItemId,
	[Required] HitType Type);