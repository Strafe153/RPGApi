using Application.Services;
using Domain.Entities;
using Domain.Interfaces.Services;

namespace WebApi.Configurations;

public static class ServicesConfiguration
{
	public static void AddCustomServices(this IServiceCollection services) =>
		services
			.AddScoped<IPlayerService, PlayerService>()
			.AddScoped<ICharacterService, CharacterService>()
			.AddScoped<IItemService<Weapon>, WeaponService>()
			.AddScoped<IItemService<Spell>, SpellService>()
			.AddScoped<IItemService<Mount>, MountService>()
			.AddSingleton<IPasswordService, PasswordService>()
			.AddSingleton<ITokenService, TokenService>()
			.AddSingleton<ICacheService, CacheService>();
}
