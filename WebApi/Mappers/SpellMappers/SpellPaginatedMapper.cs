using Domain.Dtos;
using Domain.Dtos.SpellDtos;
using Domain.Entities;
using Domain.Shared;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.SpellMappers;

public class SpellPaginatedMapper : IMapper<PaginatedList<Spell>, PageDto<SpellReadDto>>
{
	private readonly IMapper<Spell, SpellReadDto> _readMapper;

	public SpellPaginatedMapper(IMapper<Spell, SpellReadDto> readMapper)
	{
		_readMapper = readMapper;
	}

	public PageDto<SpellReadDto> Map(PaginatedList<Spell> source) => new(
		source.CurrentPage,
		source.TotalPages,
		source.PageSize,
		source.TotalItems,
		source.HasPrevious,
		source.HasNext,
		source.Select(_readMapper.Map));
}
