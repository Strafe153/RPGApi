namespace Application.Dtos.TokenDtos;

public record TokensReadDto(
    string AccessToken,
    string RefreshToken);
