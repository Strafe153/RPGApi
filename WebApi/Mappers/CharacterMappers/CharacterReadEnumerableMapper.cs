using Core.Entities;
using Core.VeiwModels.CharacterViewModels;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.CharacterMappers
{
    public class CharacterReadEnumerableMapper : IEnumerableMapper<IEnumerable<Character>, IEnumerable<CharacterReadViewModel>>
    {
        private readonly IMapper<Character, CharacterReadViewModel> _mapper;

        public CharacterReadEnumerableMapper(IMapper<Character, CharacterReadViewModel> mapper)
        {
            _mapper = mapper;
        }

        public IEnumerable<CharacterReadViewModel> Map(IEnumerable<Character> source)
        {
            var readModels = source.Select(c => _mapper.Map(c)).ToList();
            return readModels;
        }
    }
}
