using Domain.Dtos.SpellDtos;
using Domain.Entities;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.SpellMappers;

public class SpellUpdateMapper : IUpdateMapper<SpellUpdateDto, Spell>
{
	public void Map(SpellUpdateDto first, Spell second)
	{
		second.Name = first.Name;
		second.Type = first.Type;
		second.Damage = first.Damage;
	}

	public SpellUpdateDto Map(Spell second) => new(second.Name, second.Type, second.Damage);
}
