using Domain.Dtos.CharacterDtos;
using Domain.Entities;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.CharacterMappers;

public class CharacterReadMapper : IMapper<Character, CharacterReadDto>
{
	public CharacterReadDto Map(Character source)
	{
		var weapons = source.CharacterWeapons.Select(cw => cw.Weapon);
		var spells = source.CharacterSpells.Select(cs => cs.Spell);
		var mounts = source.CharacterMounts.Select(cm => cm.Mount);

		return new(
			source.Id,
			source.Name,
			source.Race,
			source.Health,
			source.PlayerId,
			weapons,
			spells,
			mounts);
	}
}
