using Core.Entities;
using Core.ViewModels.SpellViewModels;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.SpellMappers
{
    public class SpellUpdateMapper : IUpdateMapper<SpellBaseViewModel, Spell>
    {
        public void Map(SpellBaseViewModel first, Spell second)
        {
            second.Name = first.Name;
            second.Type = first.Type;
            second.Damage = first.Damage;
        }

        public SpellBaseViewModel Map(Spell second)
        {
            return new SpellBaseViewModel()
            {
                Name = second.Name,
                Type = second.Type,
                Damage = second.Damage
            };
        }
    }
}
