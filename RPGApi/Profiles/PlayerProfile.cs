using AutoMapper;
using RPGApi.Dtos;
using RPGApi.Models;

namespace RPGApi.Profiles
{
    public class PlayerProfile : Profile
    {
        public PlayerProfile()
        {
            CreateMap<Player, PlayerReadDto>();
            CreateMap<PlayerCreateUpdateDto, Player>();
        }
    }
}
