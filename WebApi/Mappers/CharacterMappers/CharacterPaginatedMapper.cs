using Core.Dtos;
using Core.Dtos.CharacterDtos;
using Core.Entities;
using Core.Models;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.CharacterMappers
{
    public class CharacterPaginatedMapper : IMapper<PaginatedList<Character>, PageDto<CharacterReadDto>>
    {
        private readonly IMapper<Character, CharacterReadDto> _readMapper;

        public CharacterPaginatedMapper(IMapper<Character, CharacterReadDto> readMapper)
        {
            _readMapper = readMapper;
        }

        public PageDto<CharacterReadDto> Map(PaginatedList<Character> source)
        {
            return new PageDto<CharacterReadDto>()
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
