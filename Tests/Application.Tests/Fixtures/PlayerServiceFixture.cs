using Application.Services;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Core.Entities;
using Core.Enums;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Models;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Security.Claims;
using System.Security.Principal;

namespace Application.Tests.Fixtures;

public class PlayerServiceFixture
{
    public PlayerServiceFixture()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

        PlayerRepository = Substitute.For<IPlayerRepository>();
        CacheService = Substitute.For<ICacheService>();
        Logger = Substitute.For<ILogger<PlayerService>>();

        PlayerService = new PlayerService(
            PlayerRepository,
            CacheService,
            Logger);

        Id = 1;
        Name = "StringPlaceholder";
        Bytes = new byte[0];
        Race = CharacterRace.Human;
        Player = GetPlayer();
        Players = GetPlayers();
        PaginatedList = GetPaginatedList();
        IIdentity = GetClaimsIdentity();
        SufficientClaims = GetSufficientClaims();
        InsufficientClaims = GetInsufficientClaims();
    }

    public IPlayerService PlayerService { get; }
    public IPlayerRepository PlayerRepository { get; }
    public ICacheService CacheService { get; }
    public ILogger<PlayerService> Logger { get; }

    public int Id { get; }
    public string? Name { get; }
    public byte[] Bytes { get; }
    public CharacterRace Race { get; }
    public Player Player { get; }
    public List<Player> Players { get; }
    public PaginatedList<Player> PaginatedList { get; }
    public IIdentity IIdentity { get; }
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

    private IIdentity GetClaimsIdentity()
    {
        return new ClaimsIdentity(Name, Name, Name);
    }

    private IEnumerable<Claim> GetInsufficientClaims()
    {
        return new List<Claim>()
        {
            new Claim(ClaimTypes.Name, Name!),
            new Claim(ClaimTypes.Role, Name!)
        };
    }

    private IEnumerable<Claim> GetSufficientClaims()
    {
        return new List<Claim>()
        {
            new Claim(ClaimTypes.Name, string.Empty),
            new Claim(ClaimTypes.Role, PlayerRole.Admin.ToString())
        };
    }
}
