using Application.Services.Abstractions;
using Application.Services.Implementations;

namespace WebApi.Configurations;

public static class ServicesConfiguration
{
	public static void AddServices(this IServiceCollection services) =>
		services
			.AddScoped<IPlayersService, PlayersService>()
			.AddScoped<ICharactersService, CharactersService>()
			.AddScoped<IWeaponsService, WeaponsService>()
			.AddScoped<ISpellsService, SpellsService>()
			.AddScoped<IMountsService, MountsService>();
}
