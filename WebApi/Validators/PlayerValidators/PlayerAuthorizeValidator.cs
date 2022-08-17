using Core.ViewModels.PlayerViewModels;
using FluentValidation;

namespace WebApi.Validators.PlayerValidators
{
    public class PlayerAuthorizeValidator : AbstractValidator<PlayerAuthorizeViewModel>
    {
        public PlayerAuthorizeValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty()
                .WithMessage("Name is required")
                .MinimumLength(2)
                .WithMessage("Name must be at least 2 characters long")
                .MaximumLength(30)
                .WithMessage("Name must not be longer than 30 characters");

            RuleFor(p => p.Password)
                .NotEmpty()
                .WithMessage("Password is required")
                .MinimumLength(6)
                .WithMessage("Password must be at least 6 characters long")
                .MaximumLength(50)
                .WithMessage("Password must not be longer than 50 characters");
        }
    }
}
