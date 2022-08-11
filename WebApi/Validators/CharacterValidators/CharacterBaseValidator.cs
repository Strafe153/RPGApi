using Core.VeiwModels.CharacterViewModels;
using FluentValidation;

namespace WebApi.Validators.CharacterValidators
{
    public class CharacterBaseValidator<T> : AbstractValidator<T> where T : CharacterBaseViewModel
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
        }
    }
}
