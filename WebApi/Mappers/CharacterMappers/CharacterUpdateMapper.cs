using Core.Entities;
using Core.ViewModels.CharacterViewModels;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.CharacterMappers
{
    public class CharacterUpdateMapper : IUpdateMapper<CharacterBaseViewModel, Character>
    {
        public void Map(CharacterBaseViewModel first, Character second)
        {
            second.Name = first.Name;
            second.Race = first.Race;
        }

        public CharacterBaseViewModel Map(Character second)
        {
            return new CharacterBaseViewModel()
            {
                Name = second.Name,
                Race = second.Race
            };
        }
    }
}
