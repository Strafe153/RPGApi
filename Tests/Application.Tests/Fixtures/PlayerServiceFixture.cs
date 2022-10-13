using Application.Services;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Core.Entities;
using Core.Enums;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Application.Tests.Fixtures;

public class PlayerServiceFixture
{
    public PlayerServiceFixture()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

        PlayerRepository = fixture.Freeze<IPlayerRepository>();
        CacheService = fixture.Freeze<ICacheService>();
        HttpContextAccessor = fixture.Freeze<IHttpContextAccessor>();
        Logger = fixture.Freeze<ILogger<PlayerService>>();

        PlayerService = new PlayerService(
            PlayerRepository,
            CacheService,
            HttpContextAccessor,
            Logger);

        Id = 1;
        Name = "ValidToken";
        Bytes = Array.Empty<byte>();
        Race = CharacterRace.Human;
        Player = GetPlayer();
        Players = GetPlayers();
        PaginatedList = GetPaginatedList();
        SufficientClaims = GetSufficientClaims();
        InsufficientClaims = GetInsufficientClaims();
    }

    public IPlayerService PlayerService { get; }
    public IPlayerRepository PlayerRepository { get; }
    public ICacheService CacheService { get; }
    public IHttpContextAccessor HttpContextAccessor { get; }
    public ILogger<PlayerService> Logger { get; }

    public int Id { get; }
    public string? Name { get; }
    public byte[] Bytes { get; }
    public CharacterRace Race { get; }
    public Player Player { get; }
    public List<Player> Players { get; }
    public PaginatedList<Player> PaginatedList { get; }
    public IEnumerable<Claim> SufficientClaims { get; }
    public IEnumerable<Claim> InsufficientClaims { get; }

    private Character GetCharacter()
    {
        return new Character()
        {
            Id = Id,
            Name = Name,
            Race = Race,
            Health = 100,
            PlayerId = Id
        };
    }

    private ICollection<Character> GetCharacters()
    {
        return new List<Character>()
        {
            GetCharacter(),
            GetCharacter()
        };
    }

    private Player GetPlayer()
    {
        return new Player()
        {
            Id = Id,
            Name = Name,
            Role = PlayerRole.Player,
            PasswordHash = Bytes,
            PasswordSalt = Bytes,
            Characters = GetCharacters()
        };
    }

    private List<Player> GetPlayers()
    {
        return new List<Player>()
        {
            Player,
            Player
        };
    }

    private PaginatedList<Player> GetPaginatedList()
    {
        return new PaginatedList<Player>(Players, 6, 1, 5);
    }

    private IEnumerable<Claim> GetInsufficientClaims()
    {
        return new List<Claim>()
        {
            new Claim(ClaimTypes.Name, string.Empty),
            new Claim(ClaimTypes.Role, string.Empty)
        };
    }

    private IEnumerable<Claim> GetSufficientClaims()
    {
        return new List<Claim>()
        {
            new Claim(ClaimTypes.Name, Name!),
            new Claim(ClaimTypes.Role, PlayerRole.Admin.ToString())
        };
    }
}
