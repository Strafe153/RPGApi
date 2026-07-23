using System.Security.Claims;
using Application.Helpers;
using Application.Services.Implementations;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace Application.Tests.Fixtures;

public class CharacterServiceFixture
{
    public Mock<IRepository<Character>> CharactersRepository { get; } = new();
    public Mock<IPlayersRepository> PlayersRepository { get; } = new();
    public Mock<IItemRepository<Weapon>> WeaponsRepository { get; } = new();
    public Mock<IItemRepository<Spell>> SpellsRepository { get; } = new();
    public Mock<IItemRepository<Mount>> MountsRepository { get; } = new();
    public Mock<ILogger<CharactersService>> Logger { get; } = new();

    public AccessHelper AccessHelper { get; private set; }

    public CharacterServiceFixture() => AccessHelper = CreateAccessHelper();

    public CharactersService CreeateSut() => new(
        CharactersRepository.Object,
        PlayersRepository.Object,
        WeaponsRepository.Object,
        SpellsRepository.Object,
        MountsRepository.Object,
        AccessHelper,
        Logger.Object);

    public void ConfigureAccessHelper(bool useSufficientClaims = true) =>
        AccessHelper = CreateAccessHelper(useSufficientClaims);

    private static IEnumerable<Claim> InsufficientClaims =>
    [
        new(ClaimTypes.Name, string.Empty),
        new(ClaimTypes.Role, string.Empty)
    ];

    private static IEnumerable<Claim> SufficientClaims =>
    [
        new (ClaimTypes.Name, "Ren"),
        new (ClaimTypes.Role, nameof(PlayerRole.Player))
    ];

    private static AccessHelper CreateAccessHelper(bool useSufficientClaims = true)
    {
        Mock<HttpContext> httpContext = new();
        Mock<IHttpContextAccessor> httpContextAccessor = new();

        var claims = useSufficientClaims ? SufficientClaims : InsufficientClaims;

        httpContext
            .Setup(c => c.User.Claims)
            .Returns(claims);

        httpContextAccessor
            .Setup(a => a.HttpContext)
            .Returns(httpContext.Object);

        return new(httpContextAccessor.Object);
    }
}
