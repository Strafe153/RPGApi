using Core.Dtos.PlayerDtos;
using Core.Entities;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.PlayerMappers;

public class PlayerReadMapper : IMapper<Player, PlayerReadDto>
{
    public PlayerReadDto Map(Player source)
    {
        return new PlayerReadDto()
        {
            Id = source.Id,
            Name = source.Name,
            Role = source.Role,
            Characters = source.Characters
        };
    }
}
