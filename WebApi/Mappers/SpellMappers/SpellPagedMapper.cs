using Core.Entities;
using Core.Models;
using Core.ViewModels;
using Core.ViewModels.SpellViewModels;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.SpellMappers
{
    public class SpellPagedMapper : IEnumerableMapper<PagedList<Spell>, PageViewModel<SpellReadViewModel>>
    {
        private readonly IMapper<Spell, SpellReadViewModel> _readMapper;

        public SpellPagedMapper(IMapper<Spell, SpellReadViewModel> readMapper)
        {
            _readMapper = readMapper;
        }

        public PageViewModel<SpellReadViewModel> Map(PagedList<Spell> source)
        {
            return new PageViewModel<SpellReadViewModel>()
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
