using Application.Dtos;
using Application.Dtos.MountDtos;
using Domain.Entities;
using Domain.Shared;

namespace Application.Mappings
{
    public static class MountMappings
    {
        public static PageDto<MountReadDto> ToPageDto(this PagedList<Mount> list)
        {
            var readDtos = list.Select(ToReadDto);

            PageDto<MountReadDto> pageDto = new(
                list.CurrentPage,
                list.TotalPages,
                list.PageSize,
                list.TotalItems,
                list.HasPrevious,
                list.HasNext,
                readDtos);

            return pageDto;
        }

        public static MountReadDto ToReadDto(this Mount entity)
        {
            var characters = entity.CharacterMounts.Select(cm => cm.Character);

            MountReadDto readDto = new(
                entity.Id,
                entity.Name,
                entity.Type,
                entity.Speed,
                characters);

            return readDto;
        }

        public static Mount ToMount(this MountCreateDto dto) => new()
        {
            Name = dto.Name,
            Type = dto.Type,
            Speed = dto.Speed
        };

        public static void Update(this MountUpdateDto dto, Mount entity)
        {
            entity.Name = dto.Name;
            entity.Type = dto.Type;
            entity.Speed = dto.Speed;
        }

        public static MountUpdateDto ToUpdateDto(this Mount entity) => new(entity.Name, entity.Type, entity.Speed);
    }
}
