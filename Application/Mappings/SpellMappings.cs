using Application.Dtos;
using Application.Dtos.SpellDtos;
using Domain.Entities;
using Domain.Shared;

namespace Application.Mappings
{
    public static class SpellMappings
    {
        public static PageDto<SpellReadDto> ToPageDto(this PagedList<Spell> list)
        {
            var readDtos = list.Select(ToReadDto);

            PageDto<SpellReadDto> pageDto = new(
                list.CurrentPage,
                list.TotalPages,
                list.PageSize,
                list.TotalItems,
                list.HasPrevious,
                list.HasNext,
                readDtos);

            return pageDto;
        }

        public static SpellReadDto ToReadDto(this Spell entity)
        {
            var characters = entity.CharacterSpells.Select(cs => cs.Character);

            SpellReadDto readDto = new(
                entity.Id,
                entity.Name,
                entity.Type,
                entity.Damage,
                characters);

            return readDto;
        }

        public static Spell ToSpell(this SpellCreateDto dto) => new()
        {
            Name = dto.Name,
            Type = dto.Type,
            Damage = dto.Damage
        };

        public static void Update(this SpellUpdateDto dto, Spell entity)
        {
            entity.Name = dto.Name;
            entity.Type = dto.Type;
            entity.Damage = dto.Damage;
        }

        public static SpellUpdateDto ToUpdateDto(this Spell entity) => new(entity.Name, entity.Type, entity.Damage);
    }
}
