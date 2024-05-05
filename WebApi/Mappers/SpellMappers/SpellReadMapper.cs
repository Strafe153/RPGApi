using Domain.Dtos.SpellDtos;
using Domain.Entities;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.SpellMappers;

public class SpellReadMapper : IMapper<Spell, SpellReadDto>
{
	public SpellReadDto Map(Spell source)
	{
		var characters = source.CharacterSpells.Select(cs => cs.Character);

		return new(
			source.Id,
			source.Name,
			source.Type,
			source.Damage,
			characters);
	}
}
