namespace Domain.Dtos;

public record HitDto(
	int DealerId,
	int ReceiverId,
	int ItemId);