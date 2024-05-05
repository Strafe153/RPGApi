using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Bogus;
using Domain.Dtos;
using Domain.Dtos.PlayerDtos;
using Domain.Dtos.TokensDtos;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Services;
using Domain.Shared;
using WebApi.Controllers.V1;
using WebApi.Mappers.Interfaces;
using WebApi.Mappers.PlayerMappers;

namespace WebApi.Tests.V1.Fixtures;

public class PlayersControllerFixture
{
    public PlayersControllerFixture()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

        Id = Random.Shared.Next();
        PlayersCount = Random.Shared.Next(1, 20);

        var playerFaker = new Faker<Player>()
            .RuleFor(p => p.Id, Id)
            .RuleFor(p => p.Name, f => f.Internet.UserName())
            .RuleFor(p => p.Role, f => (PlayerRole)f.Random.Int(Enum.GetValues(typeof(PlayerRole)).Length))
            .RuleFor(p => p.PasswordHash, f => f.Random.Bytes(32))
            .RuleFor(p => p.PasswordSalt, f => f.Random.Bytes(32))
            .RuleFor(p => p.RefreshToken, f => f.Random.String2(64));

        var playerAuthorizeDtoFaker = new Faker<PlayerAuthorizeDto>()
            .CustomInstantiator(f => new(
                f.Internet.UserName(),
                f.Internet.Password()));

        var playerUpdateDtoFaker = new Faker<PlayerUpdateDto>()
            .CustomInstantiator(f => new(f.Internet.UserName()));

        var playerChangePasswordDtoFaker = new Faker<PlayerChangePasswordDto>()
            .CustomInstantiator(f => new(f.Random.String2(16)));

        var playerChangeRoleDtoFaker = new Faker<PlayerChangeRoleDto>()
            .CustomInstantiator(f => new(f.PickRandom<PlayerRole>()));

        var tokensRefreshDtoFaker = new Faker<TokensRefreshDto>()
            .CustomInstantiator(f => new(f.Random.String2(64)));

        var pageParametersFaker = new Faker<PageParameters>()
            .RuleFor(p => p.PageNumber, f => f.Random.Int(1, 100))
            .RuleFor(p => p.PageSize, f => f.Random.Int(1, 100));

        var paginatedListFaker = new Faker<PaginatedList<Player>>()
            .CustomInstantiator(f => new(
                playerFaker.Generate(PlayersCount),
                PlayersCount,
                f.Random.Int(1, 2),
                f.Random.Int(1, 2)));

        PlayerService = fixture.Freeze<IPlayerService>();
        PasswordService = fixture.Freeze<IPasswordService>();
        TokenService = fixture.Freeze<ITokenService>();
        ReadMapper = new PlayerReadMapper();
        PaginatedMapper = new PlayerPaginatedMapper(ReadMapper);

        PlayersController = new(
            PlayerService,
            PasswordService,
            TokenService,
            PaginatedMapper,
            ReadMapper);

        Player = playerFaker.Generate();
        PlayerAuthorizeDto = playerAuthorizeDtoFaker.Generate();
        PlayerUpdateDto = playerUpdateDtoFaker.Generate();
        PlayerChangePasswordDto = playerChangePasswordDtoFaker.Generate();
        PlayerChangeRoleDto = playerChangeRoleDtoFaker.Generate();
        TokensRefreshDto = tokensRefreshDtoFaker.Generate();
        PageParameters = pageParametersFaker.Generate();
        PaginatedList = paginatedListFaker.Generate();
    }

    public PlayersController PlayersController { get; }
    public IPlayerService PlayerService { get; }
    public IPasswordService PasswordService { get; }
    public ITokenService TokenService { get; }
    public IMapper<PaginatedList<Player>, PageDto<PlayerReadDto>> PaginatedMapper { get; }
    public IMapper<Player, PlayerReadDto> ReadMapper { get; }

    public int Id { get; }
    public int PlayersCount { get; }
    public Player Player { get; }
    public TokensRefreshDto TokensRefreshDto { get; }
    public PlayerAuthorizeDto PlayerAuthorizeDto { get; }
    public PlayerUpdateDto PlayerUpdateDto { get; }
    public PlayerChangePasswordDto PlayerChangePasswordDto { get; }
    public PlayerChangeRoleDto PlayerChangeRoleDto { get; }
    public PageParameters PageParameters { get; }
    public PaginatedList<Player> PaginatedList { get; }
    public CancellationToken CancellationToken { get; }
}
