using Core.Entities;
using Core.VeiwModels.CharacterViewModels;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.CharacterMappers
{
    public class CharacterReadMapper : IMapper<Character, CharacterReadViewModel>
    {
        public CharacterReadViewModel Map(Character source)
        {
            return new CharacterReadViewModel()
            {
                Id = source.Id,
                Name = source.Name,
                Race = source.Race,
                Health = source.Health,
                PlayerId = source.PlayerId,
                Weapons = source.CharacterWeapons.Select(cw => cw.Weapon),
                Spells = source.CharacterSpells.Select(cs => cs.Spell),
                Mounts = source.CharacterMounts.Select(cm => cm.Mount)
            };
        }
    }
}
