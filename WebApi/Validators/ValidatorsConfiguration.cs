using Core.Dtos.CharacterDtos;
using Core.Dtos.MountDtos;
using Core.Dtos.PlayerDtos;
using Core.Dtos.SpellDtos;
using Core.Dtos.WeaponDtos;
using FluentValidation;
using WebApi.Validators.CharacterValidators;
using WebApi.Validators.MountValidators;
using WebApi.Validators.PlayerValidators;
using WebApi.Validators.SpellValidators;
using WebApi.Validators.WeaponValidators;

namespace WebApi.Validators;

public static class ValidatorsConfiguration
{
    public static void AddApplicationValidators(this IServiceCollection services)
    {
        // Player validators
        services.AddScoped<IValidator<PlayerBaseDto>, PlayerBaseValidator<PlayerBaseDto>>();
        services.AddScoped<IValidator<PlayerAuthorizeDto>, PlayerAuthorizeValidator>();
        services.AddScoped<IValidator<PlayerChangePasswordDto>, PlayerChangePasswordValidator>();
        services.AddScoped<IValidator<PlayerChangeRoleDto>, PlayerChangeRoleValidator>();

        // Character validators
        services.AddScoped<IValidator<CharacterBaseDto>, CharacterBaseValidator<CharacterBaseDto>>();
        services.AddScoped<IValidator<CharacterCreateDto>, CharacterCreateValidator>();

        // Weapon validators
        services.AddScoped<IValidator<WeaponBaseDto>, WeaponBaseValidator<WeaponBaseDto>>();
        services.AddScoped<IValidator<WeaponBaseDto>, WeaponCreateUpdateValidator>();

        // Spell validators
        services.AddScoped<IValidator<SpellBaseDto>, SpellBaseValidator<SpellBaseDto>>();
        services.AddScoped<IValidator<SpellBaseDto>, SpellCreateUpdateValidator>();

        // Mount validators
        services.AddScoped<IValidator<MountBaseDto>, MountBaseValidator<MountBaseDto>>();
        services.AddScoped<IValidator<MountBaseDto>, MountCreateUpdateValidator>();
    }
}
