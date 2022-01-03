using RPGApi.Dtos;
using RPGApi.Models;
using AutoMapper;

namespace RPGApi.Profiles
{
    public class WeaponProfile : Profile
    {
        public WeaponProfile()
        {
            CreateMap<Weapon, WeaponReadDto>();
            CreateMap<WeaponCreateUpdateDto, Weapon>();
        }
    }
}
