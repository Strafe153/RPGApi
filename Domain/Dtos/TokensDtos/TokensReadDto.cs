namespace Domain.Dtos.TokensDtos;

public record TokensReadDto
{
    public string? AccessToken { get; init; }
    public string? RefreshToken { get; init; }
}
