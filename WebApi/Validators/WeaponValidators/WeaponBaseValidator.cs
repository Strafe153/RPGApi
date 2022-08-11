using Core.VeiwModels.WeaponViewModels;
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
        }
    }
}
