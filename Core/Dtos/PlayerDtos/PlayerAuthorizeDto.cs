namespace Core.Dtos.PlayerDtos
{
    public record PlayerAuthorizeDto : PlayerBaseDto
    {
        public string? Password { get; init; }
    }
}
