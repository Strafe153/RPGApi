using RPGApi.Dtos;
using AutoMapper;

namespace RPGApi.Profiles
{
    public class SpellProfile : Profile
    {
        public SpellProfile()
        {
            CreateMap<Spell, SpellReadDto>();
            CreateMap<SpellCreateUpdateDto, Spell>();
            CreateMap<Spell, SpellCreateUpdateDto>();
        }
    }
}
