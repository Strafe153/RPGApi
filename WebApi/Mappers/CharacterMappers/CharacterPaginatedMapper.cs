using Core.Entities;
using Core.Models;
using Core.ViewModels;
using Core.ViewModels.CharacterViewModels;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.CharacterMappers
{
    public class CharacterPaginatedMapper : IMapper<PaginatedList<Character>, PageViewModel<CharacterReadViewModel>>
    {
        private readonly IMapper<Character, CharacterReadViewModel> _readMapper;

        public CharacterPaginatedMapper(IMapper<Character, CharacterReadViewModel> readMapper)
        {
            _readMapper = readMapper;
        }

        public PageViewModel<CharacterReadViewModel> Map(PaginatedList<Character> source)
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
