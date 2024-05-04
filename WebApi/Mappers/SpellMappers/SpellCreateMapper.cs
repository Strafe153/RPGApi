using Domain.Dtos.SpellDtos;
using Domain.Entities;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.SpellMappers;

public class SpellCreateMapper : IMapper<SpellBaseDto, Spell>
{
    public Spell Map(SpellBaseDto source)
    {
        return new Spell()
        {
            Name = source.Name,
            Type = source.Type,
            Damage = source.Damage
        };
    }
}
