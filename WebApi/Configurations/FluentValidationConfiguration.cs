using Domain.Dtos.CharacterDtos;
using Domain.Dtos.MountDtos;
using Domain.Dtos.PlayerDtos;
using Domain.Dtos.SpellDtos;
using Domain.Dtos.WeaponDtos;
using FluentValidation;
using FluentValidation.AspNetCore;
using WebApi.Validators.CharacterValidators;
using WebApi.Validators.MountValidators;
using WebApi.Validators.PlayerValidators;
using WebApi.Validators.SpellValidators;
using WebApi.Validators.WeaponValidators;

namespace WebApi.Configurations;

public static class FluentValidationConfiguration
{
	public static void ConfigureFluentValidation(this IServiceCollection services)
	{
		services.AddCustomValidators();

		services
			.AddFluentValidationAutoValidation()
			.AddFluentValidationClientsideAdapters();
	}

	private static void AddCustomValidators(this IServiceCollection services)
	{
		services
			.AddScoped<IValidator<PlayerBaseDto>, PlayerBaseValidator<PlayerBaseDto>>()
			.AddScoped<IValidator<PlayerAuthorizeDto>, PlayerAuthorizeValidator>()
			.AddScoped<IValidator<PlayerChangePasswordDto>, PlayerChangePasswordValidator>()
			.AddScoped<IValidator<PlayerChangeRoleDto>, PlayerChangeRoleValidator>();

		services
			.AddScoped<IValidator<CharacterBaseDto>, CharacterBaseValidator<CharacterBaseDto>>()
			.AddScoped<IValidator<CharacterCreateDto>, CharacterCreateValidator>();

		services
			.AddScoped<IValidator<WeaponBaseDto>, WeaponBaseValidator<WeaponBaseDto>>()
			.AddScoped<IValidator<WeaponBaseDto>, WeaponCreateUpdateValidator>();

		services
			.AddScoped<IValidator<SpellBaseDto>, SpellBaseValidator<SpellBaseDto>>()
			.AddScoped<IValidator<SpellBaseDto>, SpellCreateUpdateValidator>();

		services
			.AddScoped<IValidator<MountBaseDto>, MountBaseValidator<MountBaseDto>>()
			.AddScoped<IValidator<MountBaseDto>, MountCreateUpdateValidator>();
	}
}
