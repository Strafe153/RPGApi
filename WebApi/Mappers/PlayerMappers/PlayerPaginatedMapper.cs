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

    public PageDto<PlayerReadDto> Map(PaginatedList<Player> source)
    {
        return new PageDto<PlayerReadDto>()
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
