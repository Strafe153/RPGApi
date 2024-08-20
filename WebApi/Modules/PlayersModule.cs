using Application.Helpers;
using Application.Mappers.Abstractions;
using Application.Mappers.Implementations;
using Application.Services.Abstractions;
using Application.Services.Implementations;
using Autofac;
using DataAccess.Repositories;
using Domain.Dtos.PlayerDtos;
using Domain.Entities;
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

        builder
            .RegisterType<PlayerMapper>()
            .As<IMapper<Player, PlayerReadDto, PlayerAuthorizeDto, PlayerUpdateDto>>()
            .SingleInstance();
    }
}
