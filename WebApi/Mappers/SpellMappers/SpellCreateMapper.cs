using Domain.Dtos.SpellDtos;
using Domain.Entities;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.SpellMappers;

public class SpellCreateMapper : IMapper<SpellCreateDto, Spell>
{
    public Spell Map(SpellCreateDto source)
    {
        return new Spell()
        {
            Name = source.Name,
            Type = source.Type,
            Damage = source.Damage
        };
    }
}
