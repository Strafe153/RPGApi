﻿namespace Core.ViewModels.PlayerViewModels
{
    public record PlayerAuthorizeViewModel : PlayerBaseViewModel
    {
        public string? Password { get; init; }
    }
}
