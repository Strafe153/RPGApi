using Core.Entities;
using Core.ViewModels.SpellViewModels;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.SpellMappers
{
    public class SpellCreateMapper : IMapper<SpellBaseViewModel, Spell>
    {
        public Spell Map(SpellBaseViewModel source)
        {
            return new Spell()
            {
                Name = source.Name,
                Type = source.Type,
                Damage = source.Damage
            };
        }
    }
}
