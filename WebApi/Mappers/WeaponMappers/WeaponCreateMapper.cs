using Core.Dtos.WeaponDtos;
using Core.Entities;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.WeaponMappers;

public class WeaponCreateMapper : IMapper<WeaponBaseDto, Weapon>
{
    public Weapon Map(WeaponBaseDto source)
    {
        return new Weapon()
        {
            Name = source.Name,
            Type = source.Type,
            Damage = source.Damage
        };
    }
}
