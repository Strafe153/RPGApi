using Application.Services;
using Core.Entities;
using Core.Interfaces.Services;

namespace WebApi.Configurations;

public static class ServicesConfiguration
{
    public static void AddCustomServices(this IServiceCollection services)
    {
        services.AddScoped<IPlayerService, PlayerService>();
        services.AddScoped<ICharacterService, CharacterService>();
        services.AddScoped<IItemService<Weapon>, WeaponService>();
        services.AddScoped<IItemService<Spell>, SpellService>();
        services.AddScoped<IItemService<Mount>, MountService>();
        services.AddSingleton<IPasswordService, PasswordService>();
        services.AddSingleton<ITokenService, TokenService>();
        services.AddSingleton<ICacheService, CacheService>();
    }
}
