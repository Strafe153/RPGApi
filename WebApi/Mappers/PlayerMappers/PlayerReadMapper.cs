using Core.Entities;
using Core.ViewModels.PlayerViewModels;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.PlayerMappers
{
    public class PlayerReadMapper : IMapper<Player, PlayerReadViewModel>
    {
        public PlayerReadViewModel Map(Player source)
        {
            return new PlayerReadViewModel()
            {
                Id = source.Id,
                Name = source.Name,
                Role = source.Role,
                Characters = source.Characters
            };
        }
    }
}
