using Domain.Dtos.WeaponDtos;
using Domain.Entities;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.WeaponMappers;

public class WeaponUpdateMapper : IUpdateMapper<WeaponUpdateDto, Weapon>
{
	public void Map(WeaponUpdateDto first, Weapon second)
	{
		second.Name = first.Name;
		second.Type = first.Type;
		second.Damage = first.Damage;
	}

	public WeaponUpdateDto Map(Weapon second) => new(second.Name, second.Type, second.Damage);
}
