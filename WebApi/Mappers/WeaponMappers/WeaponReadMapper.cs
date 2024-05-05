using Domain.Dtos.WeaponDtos;
using Domain.Entities;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.WeaponMappers;

public class WeaponReadMapper : IMapper<Weapon, WeaponReadDto>
{
	public WeaponReadDto Map(Weapon source)
	{
		var characters = source.CharacterWeapons.Select(cw => cw.Character);

		return new(
			source.Id,
			source.Name,
			source.Type,
			source.Damage,
			characters);
	}
}
