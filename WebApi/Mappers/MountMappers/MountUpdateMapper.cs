using Domain.Dtos.MountDtos;
using Domain.Entities;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.MountMappers;

public class MountUpdateMapper : IUpdateMapper<MountUpdateDto, Mount>
{
	public void Map(MountUpdateDto first, Mount second)
	{
		second.Name = first.Name;
		second.Type = first.Type;
		second.Speed = first.Speed;
	}

	public MountUpdateDto Map(Mount second) => new(second.Name, second.Type, second.Speed);
}
