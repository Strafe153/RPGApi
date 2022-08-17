using Core.Entities;
using Core.Models;
using Core.ViewModels;
using Core.ViewModels.CharacterViewModels;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.CharacterMappers
{
    public class CharacterPagedMapper : IEnumerableMapper<PagedList<Character>, PageViewModel<CharacterReadViewModel>>
    {
        private readonly IMapper<Character, CharacterReadViewModel> _readMapper;

        public CharacterPagedMapper(IMapper<Character, CharacterReadViewModel> readMapper)
        {
            _readMapper = readMapper;
        }

        public PageViewModel<CharacterReadViewModel> Map(PagedList<Character> source)
        {
            return new PageViewModel<CharacterReadViewModel>()
            {
                CurrentPage = source.CurrentPage,
                TotalPages = source.TotalPages,
                PageSize = source.PageSize,
                TotalItems = source.TotalItems,
                HasPrevious = source.HasPrevious,
                HasNext = source.HasNext,
                Entities = source.Select(c => _readMapper.Map(c))
            };
        }
    }
}
