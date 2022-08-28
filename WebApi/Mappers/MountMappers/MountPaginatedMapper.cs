using Core.Entities;
using Core.Models;
using Core.ViewModels;
using Core.ViewModels.MountViewModels;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.MountMappers
{
    public class MountPaginatedMapper : IMapper<PaginatedList<Mount>, PageViewModel<MountReadViewModel>>
    {
        private readonly IMapper<Mount, MountReadViewModel> _readMapper;

        public MountPaginatedMapper(IMapper<Mount, MountReadViewModel> readMapper)
        {
            _readMapper = readMapper;
        }

        public PageViewModel<MountReadViewModel> Map(PaginatedList<Mount> source)
        {
            return new PageViewModel<MountReadViewModel>()
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
