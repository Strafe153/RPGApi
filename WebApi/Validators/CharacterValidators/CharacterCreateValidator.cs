using Core.VeiwModels.CharacterViewModels;
using FluentValidation;

namespace WebApi.Validators.CharacterValidators
{
    public class CharacterCreateValidator : CharacterBaseValidator<CharacterCreateViewModel>
    {
        public CharacterCreateValidator()
        {
            RuleFor(c => c.PlayerId)
                .NotEmpty()
                .WithMessage("PlayerId is required");
        }
    }
}
