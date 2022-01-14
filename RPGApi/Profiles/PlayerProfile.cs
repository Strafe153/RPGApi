using RPGApi.Dtos;
using AutoMapper;

namespace RPGApi.Profiles
{
    public class PlayerProfile : Profile
    {
        public PlayerProfile()
        {
            CreateMap<Player, PlayerReadDto>();
            CreateMap<PlayerUpdateDto, Player>();
            CreateMap<Player, PlayerUpdateDto>();
            CreateMap<Player, PlayerWithTokenReadDto>();
        }
    }
}
