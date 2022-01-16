using RPGApi.Dtos.Characters;
using AutoMapper;

namespace RPGApi.Profiles
{
    public class CharacterProfile : Profile
    {
        public CharacterProfile()
        {
            CreateMap<Character, CharacterReadDto>();
            CreateMap<CharacterCreateDto, Character>();
            CreateMap<CharacterUpdateDto, Character>();
            CreateMap<Character, CharacterUpdateDto>();
        }
    }
}
