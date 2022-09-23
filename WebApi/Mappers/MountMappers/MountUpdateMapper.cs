using Core.Dtos.MountDtos;
using Core.Entities;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.MountMappers;

public class MountUpdateMapper : IUpdateMapper<MountBaseDto, Mount>
{
    public void Map(MountBaseDto first, Mount second)
    {
        second.Name = first.Name;
        second.Type = first.Type;
        second.Speed = first.Speed;
    }

    public MountBaseDto Map(Mount second)
    {
        return new MountBaseDto()
        {
            Name = second.Name,
            Type = second.Type,
            Speed = second.Speed
        };
    }
}
