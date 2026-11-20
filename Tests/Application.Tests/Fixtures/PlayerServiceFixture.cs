using System.Security.Claims;
using Application.Interfaces;
using Application.Services.Implementations;
using Application.Tests.Mocks;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Security;
using Security.Implementations;

namespace Application.Tests.Fixtures;

public class PlayerServiceFixture
{
    private static readonly IOptions<JwtOptions> _jwtOptions = CreateJwtOptions();

    public Mock<IPlayersRepository> PlayersRepository { get; } = new();
    public Mock<ILogger<PlayersService>> Logger { get; } = new();

    public IAccessChecker AccessChecker { get; private set; }
    public TokenGenerator TokenGenerator { get; } = new(_jwtOptions);

    public PlayerServiceFixture() => AccessChecker = CreateAccessChecker();

    public PlayersService CreeateSut() => new(
        PlayersRepository.Object,
        AccessChecker,
        TokenGenerator,
        _jwtOptions,
        Logger.Object);

    public void ConfigureAccessHelper(bool useSufficientClaims = true) =>
        AccessChecker = CreateAccessChecker(useSufficientClaims);

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

    private static IAccessChecker CreateAccessChecker(bool useSufficientClaims = true)
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

        return new MockAccessChecker(httpContextAccessor.Object);
    }

    private static IOptions<JwtOptions> CreateJwtOptions()
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

        return options;
    }
}
