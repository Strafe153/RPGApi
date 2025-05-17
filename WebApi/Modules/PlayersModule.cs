using Application.Helpers;
using Application.Services.Abstractions;
using Application.Services.Implementations;
using Autofac;
using DataAccess.Repositories;
using Domain.Helpers;
using Domain.Repositories;

namespace WebApi.Modules;

public class PlayersModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder
            .RegisterType<PlayersRepository>()
            .As<IPlayersRepository>()
            .InstancePerLifetimeScope();

        builder
            .RegisterType<PlayersService>()
            .As<IPlayersService>()
            .InstancePerLifetimeScope();

        builder
            .RegisterType<TokenHelper>()
            .As<ITokenHelper>()
            .SingleInstance();

        builder
            .RegisterType<AccessHelper>()
            .As<IAccessHelper>()
            .SingleInstance();
    }
}
