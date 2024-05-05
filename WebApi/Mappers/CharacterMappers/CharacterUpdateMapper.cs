using Domain.Dtos.CharacterDtos;
using Domain.Entities;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.CharacterMappers;

public class CharacterUpdateMapper : IUpdateMapper<CharacterUpdateDto, Character>
{
    public void Map(CharacterUpdateDto first, Character second)
    {
        second.Name = first.Name;
        second.Race = first.Race;
    }

    public CharacterUpdateDto Map(Character second) => new(second.Name, second.Race);
}
