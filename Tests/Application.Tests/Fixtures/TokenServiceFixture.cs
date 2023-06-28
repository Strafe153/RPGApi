using Application.Services;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Bogus;
using Core.Entities;
using Core.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Application.Tests.Fixtures;

public class TokenServiceFixture
{
	public TokenServiceFixture()
	{
		var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

        var generalFaker = new Faker();

        ValidToken = generalFaker.Random.String(64);
        InvalidToken = generalFaker.Random.String(64);

        var playerwithValidTokenFaker = new Faker<Player>()
            .RuleFor(p => p.Id, f => f.Random.Int())
            .RuleFor(p => p.Name, f => f.Internet.UserName())
            .RuleFor(p => p.Role, f => (PlayerRole)f.Random.Int(Enum.GetValues(typeof(PlayerRole)).Length))
            .RuleFor(p => p.PasswordHash, f => f.Random.Bytes(32))
            .RuleFor(p => p.PasswordSalt, f => f.Random.Bytes(32))
            .RuleFor(p => p.RefreshToken, ValidToken)
            .RuleFor(p => p.RefreshTokenExpiryDate, f => f.Date.Future());

		var playerWithExpiredTokenFaker = playerwithValidTokenFaker
			.RuleFor(p => p.RefreshTokenExpiryDate, f => f.Date.Past());

        Configuration = fixture.Freeze<IConfiguration>();
		Logger = fixture.Freeze<ILogger<TokenService>>();

		TokenService = new TokenService(
			Configuration,
			Logger);

        PlayerWithValidToken = playerwithValidTokenFaker.Generate();
		PlayerWithExpiredToken = playerWithExpiredTokenFaker.Generate();
		ConfigurationSection = Substitute.For<IConfigurationSection>();
		HttpResponse = Substitute.For<HttpResponse>();
	}

	public TokenService TokenService { get; }
	public IConfiguration Configuration { get; }
	public ILogger<TokenService> Logger { get; }

	public string ValidToken { get; }
	public string InvalidToken { get; }
	public Player PlayerWithValidToken { get; }
    public Player PlayerWithExpiredToken { get; }
	public IConfigurationSection ConfigurationSection { get; }
	public HttpResponse HttpResponse { get; }
}
