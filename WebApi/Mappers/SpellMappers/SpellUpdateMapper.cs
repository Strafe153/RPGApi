using Core.Dtos.SpellDtos;
using Core.Entities;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.SpellMappers
{
    public class SpellUpdateMapper : IUpdateMapper<SpellBaseDto, Spell>
    {
        public void Map(SpellBaseDto first, Spell second)
        {
            second.Name = first.Name;
            second.Type = first.Type;
            second.Damage = first.Damage;
        }

        public SpellBaseDto Map(Spell second)
        {
            return new SpellBaseDto()
            {
                Name = second.Name,
                Type = second.Type,
                Damage = second.Damage
            };
        }
    }
}
