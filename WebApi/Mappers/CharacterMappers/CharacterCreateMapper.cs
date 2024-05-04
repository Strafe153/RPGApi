using Domain.Dtos.CharacterDtos;
using Domain.Entities;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.CharacterMappers;

public class CharacterCreateMapper : IMapper<CharacterCreateDto, Character>
{
    public Character Map(CharacterCreateDto source)
    {
        return new Character()
        {
            Name = source.Name,
            Race = source.Race,
            PlayerId = source.PlayerId
        };
    }
}
