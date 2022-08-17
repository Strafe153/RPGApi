using Core.Entities;
using Core.Models;
using Core.ViewModels;
using Core.ViewModels.MountViewModels;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.MountMappers
{
    public class MountPagedMapper : IEnumerableMapper<PagedList<Mount>, PageViewModel<MountReadViewModel>>
    {
        private readonly IMapper<Mount, MountReadViewModel> _readMapper;

        public MountPagedMapper(IMapper<Mount, MountReadViewModel> readMapper)
        {
            _readMapper = readMapper;
        }

        public PageViewModel<MountReadViewModel> Map(PagedList<Mount> source)
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
