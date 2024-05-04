﻿using Domain.Dtos.MountDtos;
using Domain.Entities;
using WebApi.Mappers.Interfaces;

namespace WebApi.Mappers.MountMappers;

public class MountCreateMapper : IMapper<MountBaseDto, Mount>
{
    public Mount Map(MountBaseDto source)
    {
        return new Mount()
        {
            Name = source.Name,
            Type = source.Type,
            Speed = source.Speed
        };
    }
}
