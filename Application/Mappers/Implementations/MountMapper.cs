using Application.Mappers.Abstractions;
using Domain.Dtos;
using Domain.Dtos.MountDtos;
using Domain.Entities;
using Domain.Shared;

namespace Application.Mappers.Implementations;

public class MountMapper : IMapper<Mount, MountReadDto, MountCreateDto, MountUpdateDto>
{
	public PageDto<MountReadDto> Map(PagedList<Mount> list) => new(
		list.CurrentPage,
		list.TotalPages,
		list.PageSize,
		list.TotalItems,
		list.HasPrevious,
		list.HasNext,
		list.Select(Map));

	public MountReadDto Map(Mount entity) => new(
		entity.Id,
		entity.Name,
		entity.Type,
		entity.Speed,
		entity.CharacterMounts.Select(cm => cm.Character));

	public Mount Map(MountCreateDto dto) => new()
	{
		Name = dto.Name,
		Type = dto.Type,
		Speed = dto.Speed
	};

	public void Map(MountUpdateDto dto, Mount entity)
	{
		entity.Name = dto.Name;
		entity.Type = dto.Type;
		entity.Speed = dto.Speed;
	}

	public MountUpdateDto MapForPatch(Mount entity) => new(entity.Name, entity.Type, entity.Speed);
}
