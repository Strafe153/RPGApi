using RPGApi.Dtos.Players;
using AutoMapper;

namespace RPGApi.Profiles
{
    public class PlayerProfile : Profile
    {
        public PlayerProfile()
        {
            CreateMap<Player, PlayerReadDto>();
            CreateMap<Player, PlayerWithTokenReadDto>();
        }
    }
}
