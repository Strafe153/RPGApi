using Application.Mappers.Abstractions;
using Domain.Dtos;
using Domain.Dtos.PlayerDtos;
using Domain.Entities;
using Domain.Enums;
using Domain.Shared;

namespace Application.Mappers.Implementations;

public class PlayerMapper : IMapper<Player, PlayerReadDto, PlayerAuthorizeDto, PlayerUpdateDto>
{
	public PageDto<PlayerReadDto> Map(PagedList<Player> list) => new(
		list.CurrentPage,
		list.TotalPages,
		list.PageSize,
		list.TotalItems,
		list.HasPrevious,
		list.HasNext,
		list.Select(Map));

	public PlayerReadDto Map(Player entity) => new(
		entity.Id,
		entity.Name,
		entity.Role,
		entity.Characters);

	public Player Map(PlayerAuthorizeDto dto) => new()
	{
		Name = dto.Name,
		Role = PlayerRole.Player,
	};

	public void Map(PlayerUpdateDto dto, Player entity) => entity.Name = dto.Name;

	public PlayerUpdateDto MapForPatch(Player entity) => throw new NotImplementedException();
}
