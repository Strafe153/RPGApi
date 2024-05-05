using Domain.Dtos.MountDtos;
using Domain.Entities;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.MountMappers;

public class MountCreateMapper : IMapper<MountCreateDto, Mount>
{
	public Mount Map(MountCreateDto source) => new()
	{
		Name = source.Name,
		Type = source.Type,
		Speed = source.Speed
	};
}
