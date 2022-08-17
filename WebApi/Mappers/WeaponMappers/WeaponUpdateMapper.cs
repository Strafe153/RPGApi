using Core.Entities;
using Core.ViewModels.WeaponViewModels;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.WeaponMappers
{
    public class WeaponUpdateMapper : IUpdateMapper<WeaponBaseViewModel, Weapon>
    {
        public void Map(WeaponBaseViewModel first, Weapon second)
        {
            second.Name = first.Name;
            second.Type = first.Type;
            second.Damage = first.Damage;
        }

        public WeaponBaseViewModel Map(Weapon second)
        {
            return new WeaponBaseViewModel()
            {
                Name = second.Name,
                Type = second.Type,
                Damage = second.Damage
            };
        }
    }
}
