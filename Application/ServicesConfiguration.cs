using Application.Services;
using Core.Entities;
using Core.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ServicesConfiguration
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IPlayerService, PlayerService>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<ICharacterService, CharacterService>();
        services.AddScoped<IItemService<Weapon>, WeaponService>();
        services.AddScoped<IItemService<Spell>, SpellService>();
        services.AddScoped<IItemService<Mount>, MountService>();
    }
}
