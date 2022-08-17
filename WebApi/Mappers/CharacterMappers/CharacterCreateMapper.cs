using Core.Entities;
using Core.ViewModels.CharacterViewModels;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.CharacterMappers
{
    public class CharacterCreateMapper : IMapper<CharacterCreateViewModel, Character>
    {
        public Character Map(CharacterCreateViewModel source)
        {
            return new Character()
            {
                Name = source.Name,
                Race = source.Race,
                PlayerId = source.PlayerId
            };
        }
    }
}
