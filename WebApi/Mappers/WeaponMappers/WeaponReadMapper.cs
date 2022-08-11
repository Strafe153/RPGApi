using Core.Entities;
using Core.VeiwModels.WeaponViewModels;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.WeaponMappers
{
    public class WeaponReadMapper : IMapper<Weapon, WeaponReadViewModel>
    {
        public WeaponReadViewModel Map(Weapon source)
        {
            return new WeaponReadViewModel()
            {
                Id = source.Id,
                Name = source.Name,
                Type = source.Type,
                Damage = source.Damage,
                Characters = source.CharacterWeapons.Select(cw => cw.Character)
            };
        }
    }
}
