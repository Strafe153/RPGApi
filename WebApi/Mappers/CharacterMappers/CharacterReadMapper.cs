using Core.Dtos.CharacterDtos;
using Core.Entities;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.CharacterMappers
{
    public class CharacterReadMapper : IMapper<Character, CharacterReadDto>
    {
        public CharacterReadDto Map(Character source)
        {
            var weapons = source.CharacterWeapons.Select(cw => cw.Weapon!);
            var spells = source.CharacterSpells.Select(cs => cs.Spell!);
            var mounts = source.CharacterMounts.Select(cm => cm.Mount!);

            return new CharacterReadDto()
            {
                Id = source.Id,
                Name = source.Name,
                Race = source.Race,
                Health = source.Health,
                PlayerId = source.PlayerId,
                Weapons = weapons,
                Spells = spells,
                Mounts = mounts
            };
        }
    }
}
