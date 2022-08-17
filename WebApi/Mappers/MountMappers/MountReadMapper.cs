using Core.Entities;
using Core.ViewModels.MountViewModels;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.MountMappers
{
    public class MountReadMapper : IMapper<Mount, MountReadViewModel>
    {
        public MountReadViewModel Map(Mount source)
        {
            var characters = source.CharacterMounts.Select(cm => cm.Character!);

            return new MountReadViewModel()
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
