using Core.Entities;
using Core.Models;
using Core.ViewModels;
using Core.ViewModels.WeaponViewModels;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.WeaponMappers
{
    public class WeaponPagedMapper : IEnumerableMapper<PagedList<Weapon>, PageViewModel<WeaponReadViewModel>>
    {
        private readonly IMapper<Weapon, WeaponReadViewModel> _readMapper;

        public WeaponPagedMapper(IMapper<Weapon, WeaponReadViewModel> readMapper)
        {
            _readMapper = readMapper;
        }

        public PageViewModel<WeaponReadViewModel> Map(PagedList<Weapon> source)
        {
            return new PageViewModel<WeaponReadViewModel>()
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
