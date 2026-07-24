using Domain.Entities;
using DataAccess.Repositories;
using Domain.Repositories;

namespace WebApi.Configurations;

public static class RepositoriesConfiguration
{
	public static void AddRepositories(this IServiceCollection services) =>
		services
			.AddScoped<IPlayersRepository, PlayersRepository>()
			.AddScoped<IRepository<Character>, CharactersRepository>()
			.AddScoped<IItemRepository<Weapon>, WeaponsRepository>()
			.AddScoped<IItemRepository<Spell>, SpellsRepository>()
			.AddScoped<IItemRepository<Mount>, MountsRepository>();
}
