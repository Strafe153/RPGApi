using Application.Dtos;
using Application.Dtos.CharactersDtos;
using Domain.Entities;
using Domain.Shared;

namespace Application.Mappings
{
    public static class CharacterMappings
    {
        public static PageDto<CharacterReadDto> ToPageDto(this PagedList<Character> list)
        {
            var readDtos = list.Select(ToReadDto);

            PageDto<CharacterReadDto> pageDto = new(
                list.CurrentPage,
                list.TotalPages,
                list.PageSize,
                list.TotalItems,
                list.HasPrevious,
                list.HasNext,
                readDtos);

            return pageDto;
        }

        public static CharacterReadDto ToReadDto(this Character entity)
        {
            var weapons = entity.CharacterWeapons.Select(cw => cw.Weapon);
            var spells = entity.CharacterSpells.Select(cs => cs.Spell);
            var mounts = entity.CharacterMounts.Select(cm => cm.Mount);

            CharacterReadDto readDto = new(
                entity.Id,
                entity.Name,
                entity.Race,
                entity.Health,
                entity.PlayerId,
                weapons,
                spells,
                mounts);

            return readDto;
        }

        public static Character ToCharacter(this CharacterCreateDto dto) => new()
        {
            Name = dto.Name,
            Race = dto.Race,
            PlayerId = dto.PlayerId
        };

        public static void Update(this CharacterUpdateDto dto, Character entity)
        {
            entity.Name = dto.Name;
            entity.Race = dto.Race;
        }

        public static CharacterUpdateDto ToUpdateDto(this Character entity) => new(entity.Name, entity.Race);
    }
}
