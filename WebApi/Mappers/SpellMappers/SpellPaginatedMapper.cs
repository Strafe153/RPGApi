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

    public PageDto<SpellReadDto> Map(PaginatedList<Spell> source)
    {
        return new PageDto<SpellReadDto>()
        {
            CurrentPage = source.CurrentPage,
            TotalPages = source.TotalPages,
            PageSize = source.PageSize,
            TotalItems = source.TotalItems,
            HasPrevious = source.HasPrevious,
            HasNext = source.HasNext,
            Entities = source.Select(p => _readMapper.Map(p))
        };
    }
}
