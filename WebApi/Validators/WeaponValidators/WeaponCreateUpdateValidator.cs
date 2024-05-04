using Domain.Dtos.WeaponDtos;
using FluentValidation;

namespace WebApi.Validators.WeaponValidators;

public class WeaponCreateUpdateValidator : WeaponBaseValidator<WeaponBaseDto>
{
    public WeaponCreateUpdateValidator()
    {
        RuleFor(w => w.Damage)
            .GreaterThan(0)
            .WithMessage("Damage must be greater than 0")
            .LessThan(101)
            .WithMessage("Damage must not be greater than 100");
    }
}
