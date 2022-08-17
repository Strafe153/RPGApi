using Core.Entities;
using Core.ViewModels.WeaponViewModels;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.WeaponMappers
{
    public class WeaponReadMapper : IMapper<Weapon, WeaponReadViewModel>
    {
        public WeaponReadViewModel Map(Weapon source)
        {
            var characters = source.CharacterWeapons.Select(cw => cw.Character!);

            return new WeaponReadViewModel()
            {
                Id = source.Id,
                Name = source.Name,
                Type = source.Type,
                Damage = source.Damage,
                Characters = characters
            };
        }
    }
}
