using Core.Entities;
using Core.ViewModels.MountViewModels;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.MountMappers
{
    public class MountUpdateMapper : IUpdateMapper<MountBaseViewModel, Mount>
    {
        public void Map(MountBaseViewModel first, Mount second)
        {
            second.Name = first.Name;
            second.Type = first.Type;
            second.Speed = first.Speed;
        }

        public MountBaseViewModel Map(Mount second)
        {
            return new MountBaseViewModel()
            {
                Name = second.Name,
                Type = second.Type,
                Speed = second.Speed
            };
        }
    }
}
