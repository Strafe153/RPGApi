using Application.Dtos;
using Application.Dtos.WeaponDtos;
using Domain.Entities;
using Domain.Shared;

namespace Application.Mappings
{
    public static class WeaponMappings
    {
        public static PageDto<WeaponReadDto> ToPageDto(this PagedList<Weapon> list)
        {
            var readDtos = list.Select(ToReadDto);

            PageDto<WeaponReadDto> pageDto = new(
                list.CurrentPage,
                list.TotalPages,
                list.PageSize,
                list.TotalItems,
                list.HasPrevious,
                list.HasNext,
                readDtos);

            return pageDto;
        }

        public static WeaponReadDto ToReadDto(this Weapon entity)
        {
            var characters = entity.CharacterWeapons.Select(cw => cw.Character);

            WeaponReadDto readDto = new(
                entity.Id,
                entity.Name,
                entity.Type,
                entity.Damage,
                characters);

            return readDto;
        }

        public static Weapon ToWeapon(this WeaponCreateDto dto) => new()
        {
            Name = dto.Name,
            Type = dto.Type,
            Damage = dto.Damage
        };

        public static void Update(this WeaponUpdateDto dto, Weapon entity)
        {
            entity.Name = dto.Name;
            entity.Type = dto.Type;
            entity.Damage = dto.Damage;
        }

        public static WeaponUpdateDto ToUpdateDto(this Weapon entity) => new(entity.Name, entity.Type, entity.Damage);
    }
}
