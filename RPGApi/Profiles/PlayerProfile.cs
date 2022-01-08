using RPGApi.Dtos;
using AutoMapper;

namespace RPGApi.Profiles
{
    public class PlayerProfile : Profile
    {
        public PlayerProfile()
        {
            CreateMap<Player, PlayerReadDto>();
            CreateMap<PlayerCreateUpdateDto, Player>();
            CreateMap<Player, PlayerCreateUpdateDto>();
        }
    }
}
