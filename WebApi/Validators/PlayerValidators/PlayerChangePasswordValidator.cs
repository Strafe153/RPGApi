using Core.ViewModels.PlayerViewModels;
using FluentValidation;

namespace WebApi.Validators.PlayerValidators
{
    public class PlayerChangePasswordValidator : AbstractValidator<PlayerChangePasswordViewModel>
    {
        public PlayerChangePasswordValidator()
        {
            RuleFor(u => u.Password)
                .NotEmpty()
                .WithMessage("Password is required")
                .MinimumLength(6)
                .WithMessage("Password must be at least 6 characters long")
                .MaximumLength(50)
                .WithMessage("Password must not be longer than 50 characters");
        }
    }
}
