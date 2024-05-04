using Domain.Dtos.MountDtos;
using Domain.Enums;
using FluentValidation;

namespace WebApi.Validators.MountValidators;

public class MountBaseValidator<T> : AbstractValidator<T> where T : MountBaseDto
{
    public MountBaseValidator()
    {
        RuleFor(w => w.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MinimumLength(2)
            .WithMessage("Name must be at least 2 characters long")
            .MaximumLength(30)
            .WithMessage("Name must not be longer than 30 characters");

        RuleFor(c => c.Type)
            .Must(BeInRange)
            .WithMessage("Type must be in the range from 0 to 10 inclusive");

        RuleFor(w => w.Speed)
            .GreaterThan(0)
            .WithMessage("Speed must be greater than 0")
            .LessThan(101)
            .WithMessage("Speed must not be greater than 100");
    }

    private bool BeInRange(MountType type)
    {
        int typeAsInt = (int)type;

        if ((typeAsInt > -1) && (typeAsInt < 11))
        {
            return true;
        }

        return false;
    }
}
