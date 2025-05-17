using Application.Services.Abstractions;
using Application.Services.Implementations;
using Autofac;
using DataAccess.Repositories;
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
    }
}
