using Domain.Dtos.PlayerDtos;
using Domain.Entities;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.PlayerMappers;

public class PlayerReadMapper : IMapper<Player, PlayerReadDto>
{
	public PlayerReadDto Map(Player source) => new(
		source.Id,
		source.Name,
		source.Role,
		source.Characters);
}
