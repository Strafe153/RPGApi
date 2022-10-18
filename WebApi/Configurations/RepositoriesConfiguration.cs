using Core.Entities;
using Core.Interfaces.Repositories;
using DataAccess.Repositories;

namespace WebApi.Configurations;

public static class RepositoriesConfiguration
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IRepository<Character>, CharacterRepository>();
        services.AddScoped<IItemRepository<Weapon>, WeaponRepository>();
        services.AddScoped<IItemRepository<Spell>, SpellRepository>();
        services.AddScoped<IItemRepository<Mount>, MountRepository>();
    }
}
