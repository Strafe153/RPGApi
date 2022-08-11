using Core.Entities;
using Core.VeiwModels.CharacterViewModels;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.CharacterMappers
{
    public class CharacterUpdateMapper : IMapperUpdater<CharacterUpdateViewModel, Character>
    {
        public void Map(CharacterUpdateViewModel first, Character second)
        {
            second.Name = first.Name;
            second.Race = first.Race;
        }

        public CharacterUpdateViewModel Map(Character second)
        {
            return new CharacterUpdateViewModel()
            {
                Name = second.Name,
                Race = second.Race
            };
        }
    }
}
