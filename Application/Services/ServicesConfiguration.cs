using Core.Entities;
using Core.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Services;

public static class ServicesConfiguration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IPlayerService, PlayerService>();
        services.AddScoped<ICharacterService, CharacterService>();
        services.AddScoped<IItemService<Weapon>, WeaponService>();
        services.AddScoped<IItemService<Spell>, SpellService>();
        services.AddScoped<IItemService<Mount>, MountService>();
        services.AddSingleton<IPasswordService, PasswordService>();
        services.AddSingleton<ITokenService, TokenService>();
        services.AddSingleton<ICacheService, CacheService>();

        return services;
    }
}
