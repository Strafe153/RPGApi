using Core.Dtos;
using Core.Dtos.MountDtos;
using Core.Entities;
using Core.Models;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.MountMappers
{
    public class MountPaginatedMapper : IMapper<PaginatedList<Mount>, PageDto<MountReadDto>>
    {
        private readonly IMapper<Mount, MountReadDto> _readMapper;

        public MountPaginatedMapper(IMapper<Mount, MountReadDto> readMapper)
        {
            _readMapper = readMapper;
        }

        public PageDto<MountReadDto> Map(PaginatedList<Mount> source)
        {
            return new PageDto<MountReadDto>()
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
