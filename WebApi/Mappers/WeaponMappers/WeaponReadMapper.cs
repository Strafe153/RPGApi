using Domain.Dtos.WeaponDtos;
using Domain.Entities;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.WeaponMappers;

public class WeaponReadMapper : IMapper<Weapon, WeaponReadDto>
{
    public WeaponReadDto Map(Weapon source)
    {
        var characters = source.CharacterWeapons.Select(cw => cw.Character!);

        return new WeaponReadDto()
        {
            Id = source.Id,
            Name = source.Name,
            Type = source.Type,
            Damage = source.Damage,
            Characters = characters
        };
    }
}
