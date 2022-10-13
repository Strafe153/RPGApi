namespace Core.Dtos.TokensDtos;

public record TokensReadDto
{
    public string? AccessToken { get; init; }
    public string? RefreshToken { get; init; }
}
