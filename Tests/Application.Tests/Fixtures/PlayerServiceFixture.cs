using Application.Services;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Bogus;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Application.Tests.Fixtures;

public class PlayerServiceFixture
{
    public PlayerServiceFixture()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

        var generalFaker = new Faker();

        Id = Random.Shared.Next();
        Name = generalFaker.Internet.UserName();
        Bytes = generalFaker.Random.Bytes(32);
        PageNumber = Random.Shared.Next(1, 25);
        PageSize = Random.Shared.Next(1, 100);
        PlayersCount = Random.Shared.Next(1, 20);

        var playerFaker = new Faker<Player>()
            .RuleFor(p => p.Id, f => f.Random.Int())
            .RuleFor(p => p.Name, Name)
            .RuleFor(p => p.Role, f => (PlayerRole)f.Random.Int(Enum.GetValues(typeof(PlayerRole)).Length))
            .RuleFor(p => p.PasswordHash, f => f.Random.Bytes(32))
            .RuleFor(p => p.PasswordSalt, f => f.Random.Bytes(32))
            .RuleFor(p => p.RefreshToken, f => f.Random.String2(64))
            .RuleFor(p => p.RefreshTokenExpiryDate, f => f.Date.Future());

        var paginatedListFaker = new Faker<PaginatedList<Player>>()
            .CustomInstantiator(f => new(
                playerFaker.Generate(PlayersCount),
                PlayersCount,
                PageNumber,
                PageSize));

        PlayerRepository = fixture.Freeze<IPlayerRepository>();
        HttpContextAccessor = fixture.Freeze<IHttpContextAccessor>();
        Logger = fixture.Freeze<ILogger<PlayerService>>();

        PlayerService = new PlayerService(PlayerRepository, HttpContextAccessor, Logger);

        Player = playerFaker.Generate();
        Players = playerFaker.Generate(PlayersCount);
        PaginatedList = paginatedListFaker.Generate();
        SufficientClaims = GetSufficientClaims();
        InsufficientClaims = GetInsufficientClaims();
    }

    private int PlayersCount { get; }

    public IPlayerService PlayerService { get; }
    public IPlayerRepository PlayerRepository { get; }
    public IHttpContextAccessor HttpContextAccessor { get; }
    public ILogger<PlayerService> Logger { get; }

    public int Id { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public string Name { get; }
    public byte[] Bytes { get; }
    public Player Player { get; }
    public List<Player> Players { get; }
    public PaginatedList<Player> PaginatedList { get; }
    public IEnumerable<Claim> SufficientClaims { get; }
    public IEnumerable<Claim> InsufficientClaims { get; }

    private IEnumerable<Claim> GetInsufficientClaims() =>
        new List<Claim>()
        {
            new Claim(ClaimTypes.Name, string.Empty),
            new Claim(ClaimTypes.Role, string.Empty)
        };

    private IEnumerable<Claim> GetSufficientClaims() =>
        new List<Claim>()
        {
            new Claim(ClaimTypes.Name, Name),
            new Claim(ClaimTypes.Role, PlayerRole.Admin.ToString())
        };
}
