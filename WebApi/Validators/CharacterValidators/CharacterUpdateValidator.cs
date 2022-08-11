using Core.VeiwModels.CharacterViewModels;
using FluentValidation;

namespace WebApi.Validators.CharacterValidators
{
    public class CharacterUpdateValidator : CharacterBaseValidator<CharacterUpdateViewModel>
    {
        public CharacterUpdateValidator()
        {
        }
    }
}
