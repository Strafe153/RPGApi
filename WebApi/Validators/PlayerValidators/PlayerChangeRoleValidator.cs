using Core.Dtos.PlayerDtos;
using Core.Enums;
using FluentValidation;

namespace WebApi.Validators.PlayerValidators;

public class PlayerChangeRoleValidator : AbstractValidator<PlayerChangeRoleDto>
{
    public PlayerChangeRoleValidator()
    {
        RuleFor(p => p.Role)
            .Must(BeInRange)
            .WithMessage("Role must be in the range from 0 to 1 inclusive");
    }

    private bool BeInRange(PlayerRole role)
    {
        int roleAsInt = (int)role;

        if ((roleAsInt > -1) && (roleAsInt < 2))
        {
            return true;
        }

        return false;
    }
}
