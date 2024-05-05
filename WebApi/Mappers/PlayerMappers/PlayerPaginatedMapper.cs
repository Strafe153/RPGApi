using Domain.Dtos;
using Domain.Dtos.PlayerDtos;
using Domain.Entities;
using Domain.Shared;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.PlayerMappers;

public class PlayerPaginatedMapper : IMapper<PaginatedList<Player>, PageDto<PlayerReadDto>>
{
	private readonly IMapper<Player, PlayerReadDto> _readMapper;

	public PlayerPaginatedMapper(IMapper<Player, PlayerReadDto> readMapper)
	{
		_readMapper = readMapper;
	}

	public PageDto<PlayerReadDto> Map(PaginatedList<Player> source) => new(
		source.CurrentPage,
		source.TotalPages,
		source.PageSize,
		source.TotalItems,
		source.HasPrevious,
		source.HasNext,
		source.Select(_readMapper.Map));
}
