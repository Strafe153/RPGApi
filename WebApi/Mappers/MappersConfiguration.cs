using Core.Entities;
using Core.Models;
using Core.ViewModels;
using Core.ViewModels.CharacterViewModels;
using Core.ViewModels.MountViewModels;
using Core.ViewModels.PlayerViewModels;
using Core.ViewModels.SpellViewModels;
using Core.ViewModels.WeaponViewModels;
using WebApi.Mappers.CharacterMappers;
using WebApi.Mappers.Interfaces;
using WebApi.Mappers.MountMappers;
using WebApi.Mappers.PlayerMappers;
using WebApi.Mappers.SpellMappers;
using WebApi.Mappers.WeaponMappers;

namespace WebApi.Mappers
{
    public static class MappersConfiguration
    {
        public static void AddApplicationMappers(this IServiceCollection services)
        {
            // Player mappers
            services.AddScoped<IMapper<PaginatedList<Player>, PageViewModel<PlayerReadViewModel>>, PlayerPaginatedMapper>();
            services.AddScoped<IMapper<Player, PlayerReadViewModel>, PlayerReadMapper>();
            services.AddScoped<IMapper<Player, PlayerWithTokenReadViewModel>, PlayerWithTokenReadMapper>();

            // Character mappers
            services.AddScoped<IMapper<PaginatedList<Character>, PageViewModel<CharacterReadViewModel>>, CharacterPaginatedMapper>();
            services.AddScoped<IMapper<Character, CharacterReadViewModel>, CharacterReadMapper>();
            services.AddScoped<IMapper<CharacterCreateViewModel, Character>, CharacterCreateMapper>();
            services.AddScoped<IUpdateMapper<CharacterBaseViewModel, Character>, CharacterUpdateMapper>();

            // Weapon mappers
            services.AddScoped<IMapper<PaginatedList<Weapon>, PageViewModel<WeaponReadViewModel>>, WeaponPaginatedMapper>();
            services.AddScoped<IMapper<Weapon, WeaponReadViewModel>, WeaponReadMapper>();
            services.AddScoped<IMapper<WeaponBaseViewModel, Weapon>, WeaponCreateMapper>();
            services.AddScoped<IUpdateMapper<WeaponBaseViewModel, Weapon>, WeaponUpdateMapper>();

            // Spell mappers
            services.AddScoped<IMapper<PaginatedList<Spell>, PageViewModel<SpellReadViewModel>>, SpellPaginatedMapper>();
            services.AddScoped<IMapper<Spell, SpellReadViewModel>, SpellReadMapper>();
            services.AddScoped<IMapper<SpellBaseViewModel, Spell>, SpellCreateMapper>();
            services.AddScoped<IUpdateMapper<SpellBaseViewModel, Spell>, SpellUpdateMapper>();

            // Mount mappers
            services.AddScoped<IMapper<PaginatedList<Mount>, PageViewModel<MountReadViewModel>>, MountPaginatedMapper>();
            services.AddScoped<IMapper<Mount, MountReadViewModel>, MountReadMapper>();
            services.AddScoped<IMapper<MountBaseViewModel, Mount>, MountCreateMapper>();
            services.AddScoped<IUpdateMapper<MountBaseViewModel, Mount>, MountUpdateMapper>();
        }
    }
}
