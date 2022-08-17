using Core.Entities;
using Core.ViewModels.PlayerViewModels;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.PlayerMappers
{
    public class PlayerWithTokenReadMapper : IMapper<Player, PlayerWithTokenReadViewModel>
    {
        public PlayerWithTokenReadViewModel Map(Player source)
        {
            return new PlayerWithTokenReadViewModel()
            {
                Id = source.Id,
                Name = source.Name,
                Role = source.Role
            };
        }
    }
}
