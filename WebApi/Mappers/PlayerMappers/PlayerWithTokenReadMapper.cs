using Core.Dtos.PlayerDtos;
using Core.Entities;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.PlayerMappers
{
    public class PlayerWithTokenReadMapper : IMapper<Player, PlayerWithTokenReadDto>
    {
        public PlayerWithTokenReadDto Map(Player source)
        {
            return new PlayerWithTokenReadDto()
            {
                Id = source.Id,
                Name = source.Name,
                Role = source.Role
            };
        }
    }
}
