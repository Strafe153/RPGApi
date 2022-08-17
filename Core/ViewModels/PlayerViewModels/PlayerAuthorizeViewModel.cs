namespace Core.ViewModels.PlayerViewModels
{
    public record PlayerAuthorizeViewModel
    {
        public string? Name { get; init; }
        public string? Password { get; init; }
    }
}
