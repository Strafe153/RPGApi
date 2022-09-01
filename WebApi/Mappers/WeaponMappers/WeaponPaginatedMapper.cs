using Core.Dtos;
using Core.Dtos.WeaponDtos;
using Core.Entities;
using Core.Models;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.WeaponMappers
{
    public class WeaponPaginatedMapper : IMapper<PaginatedList<Weapon>, PageDto<WeaponReadDto>>
    {
        private readonly IMapper<Weapon, WeaponReadDto> _readMapper;

        public WeaponPaginatedMapper(IMapper<Weapon, WeaponReadDto> readMapper)
        {
            _readMapper = readMapper;
        }

        public PageDto<WeaponReadDto> Map(PaginatedList<Weapon> source)
        {
            return new PageDto<WeaponReadDto>()
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
