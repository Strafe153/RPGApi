using Core.Entities;
using Core.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.Repositories;

public static class RepositoriesConfiguration
{
    public static IServiceCollection AddApplicationRepositories(this IServiceCollection services)
    {
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IRepository<Character>, CharacterRepository>();
        services.AddScoped<IItemRepository<Weapon>, WeaponRepository>();
        services.AddScoped<IItemRepository<Spell>, SpellRepository>();
        services.AddScoped<IItemRepository<Mount>, MountRepository>();

        return services;
    }
}
