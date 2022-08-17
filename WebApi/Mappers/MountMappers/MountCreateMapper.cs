using Core.Entities;
using Core.ViewModels.MountViewModels;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.MountMappers
{
    public class MountCreateMapper : IMapper<MountBaseViewModel, Mount>
    {
        public Mount Map(MountBaseViewModel source)
        {
            return new Mount()
            {
                Name = source.Name,
                Type = source.Type,
                Speed = source.Speed
            };
        }
    }
}
