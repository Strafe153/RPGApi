using Core.Enums;
using Core.ViewModels.WeaponViewModels;
using FluentValidation;

namespace WebApi.Validators.WeaponValidators
{
    public class WeaponBaseValidator<T> : AbstractValidator<T> where T : WeaponBaseViewModel
    {
        public WeaponBaseValidator()
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
                .WithMessage("Type must be in the range from 0 to 15 inclusive");

            RuleFor(w => w.Damage)
                .GreaterThan(0)
                .WithMessage("Damage must be greater than 0")
                .LessThan(101)
                .WithMessage("Damage must not be greater than 100");
        }

        private bool BeInRange(WeaponType type)
        {
            int typeAsInt = (int)type;

            if ((typeAsInt > -1) && (typeAsInt < 16))
            {
                return true;
            }

            return false;
        }
    }
}
