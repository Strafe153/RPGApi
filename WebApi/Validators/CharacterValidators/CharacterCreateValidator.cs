using Domain.Dtos.CharacterDtos;
using FluentValidation;

namespace WebApi.Validators.CharacterValidators;

public class CharacterCreateValidator : CharacterBaseValidator<CharacterCreateDto>
{
    public CharacterCreateValidator()
    {
        RuleFor(c => c.PlayerId)
            .NotEmpty()
            .WithMessage("PlayerId is required");
    }
}
