namespace Domain.Dtos.TokensDtos;

public record TokensReadDto(
	string AccessToken,
	string RefreshToken);