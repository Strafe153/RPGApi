using Domain.Dtos;
using Domain.Dtos.WeaponDtos;
using Domain.Entities;
using Domain.Shared;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.WeaponMappers;

public class WeaponPaginatedMapper : IMapper<PaginatedList<Weapon>, PageDto<WeaponReadDto>>
{
	private readonly IMapper<Weapon, WeaponReadDto> _readMapper;

	public WeaponPaginatedMapper(IMapper<Weapon, WeaponReadDto> readMapper)
	{
		_readMapper = readMapper;
	}

	public PageDto<WeaponReadDto> Map(PaginatedList<Weapon> source) => new(
		source.CurrentPage,
		source.TotalPages,
		source.PageSize,
		source.TotalItems,
		source.HasPrevious,
		source.HasNext,
		source.Select(_readMapper.Map));
}
