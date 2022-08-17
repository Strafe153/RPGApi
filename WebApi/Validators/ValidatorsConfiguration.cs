using Core.ViewModels.CharacterViewModels;
using Core.ViewModels.MountViewModels;
using Core.ViewModels.PlayerViewModels;
using Core.ViewModels.SpellViewModels;
using Core.ViewModels.WeaponViewModels;
using FluentValidation;
using WebApi.Validators.CharacterValidators;
using WebApi.Validators.MountValidators;
using WebApi.Validators.PlayerValidators;
using WebApi.Validators.SpellValidators;
using WebApi.Validators.WeaponValidators;

namespace WebApi.Validators
{
    public static class ValidatorsConfiguration
    {
        public static void AddApplicationValidators(this IServiceCollection services)
        {
            // Player validators
            services.AddScoped<IValidator<PlayerAuthorizeViewModel>, PlayerAuthorizeValidator>();
            services.AddScoped<IValidator<PlayerChangeRoleViewModel>, PlayerChangeRoleValidator>();

            // Character validators
            services.AddScoped<IValidator<CharacterBaseViewModel>, CharacterBaseValidator<CharacterBaseViewModel>>();
            services.AddScoped<IValidator<CharacterCreateViewModel>, CharacterCreateValidator>();

            // Weapon validators
            services.AddScoped<IValidator<WeaponBaseViewModel>, WeaponBaseValidator<WeaponBaseViewModel>>();
            services.AddScoped<IValidator<WeaponBaseViewModel>, WeaponCreateUpdateValidator>();

            // Spell validators
            services.AddScoped<IValidator<SpellBaseViewModel>, SpellBaseValidator<SpellBaseViewModel>>();
            services.AddScoped<IValidator<SpellBaseViewModel>, SpellCreateUpdateValidator>();

            // Mount validators
            services.AddScoped<IValidator<MountBaseViewModel>, MountBaseValidator<MountBaseViewModel>>();
            services.AddScoped<IValidator<MountBaseViewModel>, MountCreateUpdateValidator>();
        }
    }
}
