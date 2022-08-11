using Core.VeiwModels.CharacterViewModels;
using Core.VeiwModels.WeaponViewModels;
using FluentValidation;
using WebApi.Validators.CharacterValidators;
using WebApi.Validators.WeaponValidators;

namespace WebApi.Validators
{
    public static class ValidatorsConfiguration
    {
        public static void AddApplicationValidators(this IServiceCollection services)
        {
            services.AddScoped<IValidator<CharacterBaseViewModel>, CharacterBaseValidator<CharacterBaseViewModel>>();
            services.AddScoped<IValidator<CharacterCreateViewModel>, CharacterCreateValidator>();
            services.AddScoped<IValidator<CharacterUpdateViewModel>, CharacterUpdateValidator>();

            services.AddScoped<IValidator<WeaponBaseViewModel>, WeaponBaseValidator<WeaponBaseViewModel>>();
            services.AddScoped<IValidator<WeaponCreateUpdateViewModel>, WeaponCreateUpdateValidator>();
        }
    }
}
