using Core.Entities;
using Core.VeiwModels.CharacterViewModels;
using Core.VeiwModels.WeaponViewModels;
using WebApi.Mappers.CharacterMappers;
using WebApi.Mappers.Interfaces;
using WebApi.Mappers.WeaponMappers;

namespace WebApi.Mappers
{
    public static class MappersConfiguration
    {
        public static void AddApplicationMappers(this IServiceCollection services)
        {
            services.AddScoped<IEnumerableMapper<IEnumerable<Character>, IEnumerable<CharacterReadViewModel>>,
                CharacterReadEnumerableMapper>();
            services.AddScoped<IMapper<Character, CharacterReadViewModel>, CharacterReadMapper>();
            services.AddScoped<IMapper<CharacterCreateViewModel, Character>, CharacterCreateMapper>();
            services.AddScoped<IMapperUpdater<CharacterUpdateViewModel, Character>, CharacterUpdateMapper>();

            services.AddScoped<IEnumerableMapper<IEnumerable<Weapon>, IEnumerable<WeaponReadViewModel>>,
                WeaponReadEnumerableMapper>();
            services.AddScoped<IMapper<Weapon, WeaponReadViewModel>, WeaponReadMapper>();
        }
    }
}
