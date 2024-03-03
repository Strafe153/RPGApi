using Core.Entities;
using Core.Interfaces.Repositories;
using DataAccess.Repositories;

namespace WebApi.Configurations;

public static class RepositoriesConfiguration
{
	public static void AddRepositories(this IServiceCollection services) =>
		services
			.AddScoped<IPlayerRepository, PlayerRepository>()
			.AddScoped<IRepository<Character>, CharacterRepository>()
			.AddScoped<IItemRepository<Weapon>, WeaponRepository>()
			.AddScoped<IItemRepository<Spell>, SpellRepository>()
			.AddScoped<IItemRepository<Mount>, MountRepository>();
}
