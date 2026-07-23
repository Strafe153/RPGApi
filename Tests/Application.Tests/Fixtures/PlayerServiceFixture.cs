using System.Security.Claims;
using Application.Helpers;
using Application.Services.Implementations;
using Domain.Enums;
using Domain.Helpers;
using Domain.Repositories;
using Domain.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Application.Tests.Fixtures;

public class PlayerServiceFixture
{
    public Mock<IPlayersRepository> PlayersRepository { get; } = new();
    public Mock<ILogger<PlayersService>> Logger { get; } = new();

    public AccessHelper AccessHelper { get; private set; }
    public TokenHelper TokenHelper { get; } = CreateTokenHelper();

    public PlayerServiceFixture() => AccessHelper = CreateAccessHelper();

    public PlayersService CreeateSut() => new(
        PlayersRepository.Object,
        AccessHelper,
        TokenHelper,
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

    private static TokenHelper CreateTokenHelper()
    {
        JwtOptions jwtOptions = new()
        {
            Secret = "iTXRmAO8n7b9piCEv7PcVcUrRkjXmT7r3zxoofyHyYtPw199cHWTvKuDAe3cVQzh",
            Issuer = "https://localhost:5001",
            Audience = "https://localhost:5001",
            AccessExpirationPeriod = 15,
            RefreshExpirationPeriod = 30
        };

        var options = Options.Create(jwtOptions);
        Mock<ILogger<TokenHelper>> logger = new();

        return new TokenHelper(options, logger.Object);
    }
}
