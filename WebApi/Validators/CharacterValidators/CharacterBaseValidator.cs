using Domain.Dtos.CharacterDtos;
using Domain.Enums;
using FluentValidation;

namespace WebApi.Validators.CharacterValidators;

public class CharacterBaseValidator<T> : AbstractValidator<T> where T : CharacterBaseDto
{
    public CharacterBaseValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MinimumLength(2)
            .WithMessage("Name must be at least 2 characters long")
            .MaximumLength(30)
            .WithMessage("Name must not be longer than 30 characters");

        RuleFor(c => c.Race)
            .Must(BeInRange)
            .WithMessage("Role must be in the range from 0 to 20 inclusive");
    }

    private bool BeInRange(CharacterRace race)
    {
        int raceAsInt = (int)race;

        if ((raceAsInt > -1) && (raceAsInt < 21))
        {
            return true;
        }

        return false;
    }
}
