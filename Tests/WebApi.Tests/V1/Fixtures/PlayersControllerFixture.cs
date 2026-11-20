using Application.Services.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Security;
using Security.Implementations;
using WebApi.Controllers.V1;

namespace WebApi.Tests.V1.Fixtures;

public class PlayersControllerFixture
{
    public Mock<IPlayersService> PlayersService { get; } = new();

    public TokenGenerator TokenGenerator { get; } = CreateTokenHelper();

    public PlayersController CreateSut() => new(PlayersService.Object);

    private static TokenGenerator CreateTokenHelper()
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

        return new TokenGenerator(options);
    }
}
