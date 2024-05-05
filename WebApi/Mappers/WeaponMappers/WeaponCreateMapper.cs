using Domain.Dtos.WeaponDtos;
using Domain.Entities;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.WeaponMappers;

public class WeaponCreateMapper : IMapper<WeaponCreateDto, Weapon>
{
    public Weapon Map(WeaponCreateDto source)
    {
        return new Weapon()
        {
            Name = source.Name,
            Type = source.Type,
            Damage = source.Damage
        };
    }
}
