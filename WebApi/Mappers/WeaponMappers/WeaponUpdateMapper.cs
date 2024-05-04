using Domain.Dtos.WeaponDtos;
using Domain.Entities;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.WeaponMappers;

public class WeaponUpdateMapper : IUpdateMapper<WeaponBaseDto, Weapon>
{
    public void Map(WeaponBaseDto first, Weapon second)
    {
        second.Name = first.Name;
        second.Type = first.Type;
        second.Damage = first.Damage;
    }

    public WeaponBaseDto Map(Weapon second)
    {
        return new WeaponBaseDto()
        {
            Name = second.Name,
            Type = second.Type,
            Damage = second.Damage
        };
    }
}
