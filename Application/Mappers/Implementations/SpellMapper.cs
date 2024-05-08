using Application.Mappers.Abstractions;
using Domain.Dtos;
using Domain.Dtos.SpellDtos;
using Domain.Entities;
using Domain.Shared;

namespace Application.Mappers.Implementations;

public class SpellMapper : IMapper<Spell, SpellReadDto, SpellCreateDto, SpellUpdateDto>
{
	public PageDto<SpellReadDto> Map(PagedList<Spell> list) => new(
		list.CurrentPage,
		list.TotalPages,
		list.PageSize,
		list.TotalItems,
		list.HasPrevious,
		list.HasNext,
		list.Select(Map));

	public SpellReadDto Map(Spell entity) => new(
		entity.Id,
		entity.Name,
		entity.Type,
		entity.Damage,
		entity.CharacterSpells.Select(cs => cs.Character));

	public Spell Map(SpellCreateDto dto) => new()
	{
		Name = dto.Name,
		Type = dto.Type,
		Damage = dto.Damage
	};

	public void Map(SpellUpdateDto dto, Spell entity)
	{
		entity.Name = dto.Name;
		entity.Type = dto.Type;
		entity.Damage = dto.Damage;
	}

	public SpellUpdateDto MapForPatch(Spell entity) => new(entity.Name, entity.Type, entity.Damage);
}
