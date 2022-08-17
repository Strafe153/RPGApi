using Core.Entities;
using Core.ViewModels.SpellViewModels;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.SpellMappers
{
    public class SpellReadMapper : IMapper<Spell, SpellReadViewModel>
    {
        public SpellReadViewModel Map(Spell source)
        {
            var characters = source.CharacterSpells.Select(cs => cs.Character!);

            return new SpellReadViewModel()
            {
                Id = source.Id,
                Name = source.Name,
                Type = source.Type,
                Damage = source.Damage,
                Characters = characters
            };
        }
    }
}
