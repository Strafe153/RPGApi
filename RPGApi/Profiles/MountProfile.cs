using RPGApi.Dtos;
using RPGApi.Models;
using AutoMapper;

namespace RPGApi.Profiles
{
    public class MountProfile : Profile
    {
        public MountProfile()
        {
            CreateMap<Mount, MountReadDto>();
            CreateMap<MountCreateUpdateDto, Mount>();
            CreateMap<Mount, MountCreateUpdateDto>();
        }
    }
}
