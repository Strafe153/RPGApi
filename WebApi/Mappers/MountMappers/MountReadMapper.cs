using Core.Dtos.MountDtos;
using Core.Entities;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.MountMappers
{
    public class MountReadMapper : IMapper<Mount, MountReadDto>
    {
        public MountReadDto Map(Mount source)
        {
            var characters = source.CharacterMounts.Select(cm => cm.Character!);

            return new MountReadDto()
            {
                Id = source.Id,
                Name = source.Name,
                Type = source.Type,
                Speed = source.Speed,
                Characters = characters
            };
        }
    }
}
