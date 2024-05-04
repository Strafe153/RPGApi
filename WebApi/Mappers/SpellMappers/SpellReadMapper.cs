using Domain.Dtos.SpellDtos;
using Domain.Entities;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.SpellMappers;

public class SpellReadMapper : IMapper<Spell, SpellReadDto>
{
    public SpellReadDto Map(Spell source)
    {
        var characters = source.CharacterSpells.Select(cs => cs.Character!);

        return new SpellReadDto()
        {
            Id = source.Id,
            Name = source.Name,
            Type = source.Type,
            Damage = source.Damage,
            Characters = characters
        };
    }
}
