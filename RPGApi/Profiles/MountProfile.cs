using RPGApi.Dtos.Mounts;
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
