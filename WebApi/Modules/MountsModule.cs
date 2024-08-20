using Application.Mappers.Abstractions;
using Application.Mappers.Implementations;
using Application.Services.Abstractions;
using Application.Services.Implementations;
using Autofac;
using DataAccess.Repositories;
using Domain.Dtos.MountDtos;
using Domain.Entities;
using Domain.Repositories;

namespace WebApi.Modules;

public class MountsModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder
            .RegisterType<MountsRepository>()
            .As<IItemRepository<Mount>>()
            .InstancePerLifetimeScope();

        builder
            .RegisterType<MountsService>()
            .As<IMountsService>()
            .InstancePerLifetimeScope();

        builder
            .RegisterType<MountMapper>()
            .As<IMapper<Mount, MountReadDto, MountCreateDto, MountUpdateDto>>()
            .SingleInstance();
    }
}
