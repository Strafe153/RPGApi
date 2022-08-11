using Core.Entities;
using Core.VeiwModels.WeaponViewModels;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.WeaponMappers
{
    public class WeaponReadEnumerableMapper : IEnumerableMapper<IEnumerable<Weapon>, IEnumerable<WeaponReadViewModel>>
    {
        private readonly IMapper<Weapon, WeaponReadViewModel> _mapper;

        public WeaponReadEnumerableMapper(IMapper<Weapon, WeaponReadViewModel> mapper)
        {
            _mapper = mapper;
        }

        public IEnumerable<WeaponReadViewModel> Map(IEnumerable<Weapon> source)
        {
            var readModels = source.Select(w => _mapper.Map(w)).ToList();
            return readModels;
        }
    }
}
