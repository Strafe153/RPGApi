using Application.Mappers.Abstractions;
using Domain.Dtos;
using Domain.Dtos.CharacterDtos;
using Domain.Entities;
using Domain.Shared;

namespace Application.Mappers.Implementations;

public class CharacterMapper : IMapper<Character, CharacterReadDto, CharacterCreateDto, CharacterUpdateDto>
{
	public PageDto<CharacterReadDto> Map(PagedList<Character> list) => new(
		list.CurrentPage,
		list.TotalPages,
		list.PageSize,
		list.TotalItems,
		list.HasPrevious,
		list.HasNext,
		list.Select(Map));

	public CharacterReadDto Map(Character entity) => new(
		entity.Id,
		entity.Name,
		entity.Race,
		entity.Health,
		entity.PlayerId,
		entity.CharacterWeapons.Select(cw => cw.Weapon),
		entity.CharacterSpells.Select(cs => cs.Spell),
		entity.CharacterMounts.Select(cm => cm.Mount));

	public Character Map(CharacterCreateDto dto) => new()
	{
		Name = dto.Name,
		Race = dto.Race,
		PlayerId = dto.PlayerId
	};

	public void Map(CharacterUpdateDto dto, Character entity)
	{
		entity.Name = dto.Name;
		entity.Race = dto.Race;
	}

	public CharacterUpdateDto MapForPatch(Character entity) => new(entity.Name, entity.Race);
}
