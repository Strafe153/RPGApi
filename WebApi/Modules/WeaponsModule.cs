using Application.Mappers.Abstractions;
using Application.Mappers.Implementations;
using Application.Services.Abstractions;
using Application.Services.Implementations;
using Autofac;
using DataAccess.Repositories;
using Domain.Dtos.WeaponDtos;
using Domain.Entities;
using Domain.Repositories;

namespace WebApi.Modules;

public class WeaponsModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder
            .RegisterType<WeaponsRepository>()
            .As<IItemRepository<Weapon>>()
            .InstancePerLifetimeScope();

        builder
            .RegisterType<WeaponsService>()
            .As<IWeaponsService>()
            .InstancePerLifetimeScope();

        builder
            .RegisterType<WeaponMapper>()
            .As<IMapper<Weapon, WeaponReadDto, WeaponCreateDto, WeaponUpdateDto>>()
            .SingleInstance();
    }
}
