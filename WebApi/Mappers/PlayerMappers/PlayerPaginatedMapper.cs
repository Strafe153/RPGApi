using Core.Entities;
using Core.Models;
using Core.ViewModels;
using Core.ViewModels.PlayerViewModels;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.PlayerMappers
{
    public class PlayerPaginatedMapper : IMapper<PaginatedList<Player>, PageViewModel<PlayerReadViewModel>>
    {
        private readonly IMapper<Player, PlayerReadViewModel> _readMapper;

        public PlayerPaginatedMapper(IMapper<Player, PlayerReadViewModel> readMapper)
        {
            _readMapper = readMapper;
        }

        public PageViewModel<PlayerReadViewModel> Map(PaginatedList<Player> source)
        {
            return new PageViewModel<PlayerReadViewModel>()
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
}
