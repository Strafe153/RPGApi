using Application.Mappers.Abstractions;
using Application.Mappers.Implementations;
using Application.Services.Abstractions;
using Application.Services.Implementations;
using Autofac;
using DataAccess.Repositories;
using Domain.Dtos.CharacterDtos;
using Domain.Entities;
using Domain.Repositories;

namespace WebApi.Modules;

public class CharactersModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder
            .RegisterType<CharactersRepository>()
            .As<IRepository<Character>>()
            .InstancePerLifetimeScope();

        builder
            .RegisterType<CharactersService>()
            .As<ICharactersService>()
            .InstancePerLifetimeScope();

        builder
            .RegisterType<CharacterMapper>()
            .As<IMapper<Character, CharacterReadDto, CharacterCreateDto, CharacterUpdateDto>>()
            .SingleInstance();
    }
}
