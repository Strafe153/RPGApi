using Domain.Dtos.MountDtos;
using Domain.Entities;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.MountMappers;

public class MountReadMapper : IMapper<Mount, MountReadDto>
{
	public MountReadDto Map(Mount source)
	{
		var characters = source.CharacterMounts.Select(cm => cm.Character);

		return new(
			source.Id,
			source.Name,
			source.Type,
			source.Speed,
			characters);
	}
}
