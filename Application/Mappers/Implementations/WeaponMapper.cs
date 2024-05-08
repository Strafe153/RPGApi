using Application.Mappers.Abstractions;
using Domain.Dtos;
using Domain.Dtos.WeaponDtos;
using Domain.Entities;
using Domain.Shared;

namespace Application.Mappers.Implementations;

public class WeaponMapper : IMapper<Weapon, WeaponReadDto, WeaponCreateDto, WeaponUpdateDto>
{
	public PageDto<WeaponReadDto> Map(PagedList<Weapon> list) => new(
		list.CurrentPage,
		list.TotalPages,
		list.PageSize,
		list.TotalItems,
		list.HasPrevious,
		list.HasNext,
		list.Select(Map));

	public WeaponReadDto Map(Weapon entity) => new(
		entity.Id,
		entity.Name,
		entity.Type,
		entity.Damage,
		entity.CharacterWeapons.Select(cw => cw.Character));

	public Weapon Map(WeaponCreateDto dto) => new()
	{
		Name = dto.Name,
		Type = dto.Type,
		Damage = dto.Damage
	};

	public void Map(WeaponUpdateDto dto, Weapon entity)
	{
		entity.Name = dto.Name;
		entity.Type = dto.Type;
		entity.Damage = dto.Damage;
	}

	public WeaponUpdateDto MapForPatch(Weapon entity) => new(entity.Name, entity.Type, entity.Damage);
}
