using Application.Helpers;
using Application.Services.Abstractions;
using Domain.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using WebApi.Controllers.V1;

namespace WebApi.Tests.V1.Fixtures;

public class PlayersControllerFixture
{
    public Mock<IPlayersService> PlayersService { get; } = new();

    public TokenHelper TokenHelper { get; } = CreateTokenHelper();

    public PlayersController CreateSut() => new(PlayersService.Object);

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
