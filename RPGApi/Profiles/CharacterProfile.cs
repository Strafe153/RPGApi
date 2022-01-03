using RPGApi.Dtos;
using RPGApi.Models;
using AutoMapper;

namespace RPGApi.Profiles
{
    public class CharacterProfile : Profile
    {
        public CharacterProfile()
        {
            CreateMap<Character, CharacterReadDto>();
            CreateMap<CharacterCreateUpdateDto, Character>();
        }
    }
}
