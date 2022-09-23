using Core.Dtos.CharacterDtos;
using Core.Entities;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.CharacterMappers;

public class CharacterUpdateMapper : IUpdateMapper<CharacterBaseDto, Character>
{
    public void Map(CharacterBaseDto first, Character second)
    {
        second.Name = first.Name;
        second.Race = first.Race;
    }

    public CharacterBaseDto Map(Character second)
    {
        return new CharacterBaseDto()
        {
            Name = second.Name,
            Race = second.Race
        };
    }
}
