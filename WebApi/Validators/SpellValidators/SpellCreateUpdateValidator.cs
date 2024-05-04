using Domain.Dtos.SpellDtos;
using FluentValidation;

namespace WebApi.Validators.SpellValidators;

public class SpellCreateUpdateValidator : SpellBaseValidator<SpellBaseDto>
{
    public SpellCreateUpdateValidator()
    {
        RuleFor(w => w.Damage)
            .GreaterThan(0)
            .WithMessage("Damage must be greater than 0")
            .LessThan(101)
            .WithMessage("Damage must not be greater than 100");
    }
}
