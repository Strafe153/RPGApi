using Core.Entities;
using Core.ViewModels.WeaponViewModels;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.WeaponMappers
{
    public class WeaponCreateMapper : IMapper<WeaponBaseViewModel, Weapon>
    {
        public Weapon Map(WeaponBaseViewModel source)
        {
            return new Weapon()
            {
                Name = source.Name,
                Type = source.Type,
                Damage = source.Damage
            };
        }
    }
}
