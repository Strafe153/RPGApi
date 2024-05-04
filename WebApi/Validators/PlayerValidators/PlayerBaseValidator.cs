using Domain.Dtos.PlayerDtos;
using FluentValidation;

namespace WebApi.Validators.PlayerValidators;

public class PlayerBaseValidator<T> : AbstractValidator<T> where T : PlayerBaseDto
{
    public PlayerBaseValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MinimumLength(2)
            .WithMessage("Name must be at least 2 characters long")
            .MaximumLength(30)
            .WithMessage("Name must not be longer than 30 characters");
    }
}
