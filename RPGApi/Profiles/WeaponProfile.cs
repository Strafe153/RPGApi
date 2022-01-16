using RPGApi.Dtos.Weapons;
using AutoMapper;

namespace RPGApi.Profiles
{
    public class WeaponProfile : Profile
    {
        public WeaponProfile()
        {
            CreateMap<Weapon, WeaponReadDto>();
            CreateMap<WeaponCreateUpdateDto, Weapon>();
            CreateMap<Weapon, WeaponCreateUpdateDto>();
        }
    }
}
