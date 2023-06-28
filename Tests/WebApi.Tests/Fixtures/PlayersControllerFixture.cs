using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Bogus;
using Core.Dtos;
using Core.Dtos.PlayerDtos;
using Core.Dtos.TokensDtos;
using Core.Entities;
using Core.Enums;
using Core.Interfaces.Services;
using Core.Shared;
using WebApi.Controllers;
using WebApi.Mappers.Interfaces;
using WebApi.Mappers.PlayerMappers;

namespace WebApi.Tests.Fixtures;

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
            .RuleFor(p => p.RefreshToken, f => f.Random.String(64));

        var playerAuthorizeDtoFaker = new Faker<PlayerAuthorizeDto>()
            .RuleFor(p => p.Name, f => f.Internet.UserName())
            .RuleFor(p => p.Password, f => f.Internet.Password());

        var playerUpdateDtoFaker = new Faker<PlayerBaseDto>()
            .RuleFor(p => p.Name, f => f.Internet.UserName());

        var playerChangePasswordDtoFaker = new Faker<PlayerChangePasswordDto>()
            .RuleFor(p => p.Password, f => f.Random.String(16));

        var playerChangeRoleDtoFaker = new Faker<PlayerChangeRoleDto>()
            .RuleFor(p => p.Role, f => (PlayerRole)f.Random.Int(Enum.GetValues(typeof(PlayerRole)).Length));

        var tokensRefreshDtoFaker = new Faker<TokensRefreshDto>()
            .RuleFor(t => t.RefreshToken, f => f.Random.String(64));

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

        PlayersController = new PlayersController(
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
    public PlayerBaseDto PlayerUpdateDto { get; }
    public PlayerChangePasswordDto PlayerChangePasswordDto { get; }
    public PlayerChangeRoleDto PlayerChangeRoleDto { get; }
    public PageParameters PageParameters { get; }
    public PaginatedList<Player> PaginatedList { get; }
    public CancellationToken CancellationToken { get; }
}
