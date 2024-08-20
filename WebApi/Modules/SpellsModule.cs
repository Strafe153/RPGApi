using Application.Mappers.Abstractions;
using Application.Mappers.Implementations;
using Application.Services.Abstractions;
using Application.Services.Implementations;
using Autofac;
using DataAccess.Repositories;
using Domain.Dtos.SpellDtos;
using Domain.Entities;
using Domain.Repositories;

namespace WebApi.Modules;

public class SpellsModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder
            .RegisterType<SpellsRepository>()
            .As<IItemRepository<Spell>>()
            .InstancePerLifetimeScope();

        builder
            .RegisterType<SpellsService>()
            .As<ISpellsService>()
            .InstancePerLifetimeScope();

        builder
            .RegisterType<SpellMapper>()
            .As<IMapper<Spell, SpellReadDto, SpellCreateDto, SpellUpdateDto>>()
            .SingleInstance();
    }
}
