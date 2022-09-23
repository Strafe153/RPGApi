using Core.Entities;
using Core.Interfaces.Repositories;
using DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess;

public static class RepositoriesConfiguration
{
    public static void AddApplicationRepositories(this IServiceCollection services)
    {
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IRepository<Character>, CharacterRepository>();
        services.AddScoped<IRepository<Weapon>, WeaponRepository>();
        services.AddScoped<IRepository<Spell>, SpellRepository>();
        services.AddScoped<IRepository<Mount>, MountRepository>();
    }
}
