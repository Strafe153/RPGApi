using Domain.Dtos;
using Domain.Dtos.MountDtos;
using Domain.Entities;
using Domain.Shared;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.MountMappers;

public class MountPaginatedMapper : IMapper<PaginatedList<Mount>, PageDto<MountReadDto>>
{
	private readonly IMapper<Mount, MountReadDto> _readMapper;

	public MountPaginatedMapper(IMapper<Mount, MountReadDto> readMapper)
	{
		_readMapper = readMapper;
	}

	public PageDto<MountReadDto> Map(PaginatedList<Mount> source) => new(
		source.CurrentPage,
		source.TotalPages,
		source.PageSize,
		source.TotalItems,
		source.HasPrevious,
		source.HasNext,
		source.Select(_readMapper.Map));
}
