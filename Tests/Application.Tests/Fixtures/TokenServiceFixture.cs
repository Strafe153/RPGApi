using Application.Services;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
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

		Configuration = fixture.Freeze<IConfiguration>();
		Logger = fixture.Freeze<ILogger<TokenService>>();

		TokenService = new TokenService(
			Configuration,
			Logger);

		Bytes = Array.Empty<byte>();
		ValidToken = "valid-refresh-token";
		InvalidToken = "invalid-refresh-token";
        PlayerWithValidToken = GetPlayerWithValidToken();
		PlayerWithExpiredToken = GetPlayerWithExpiredToken();
		ConfigurationSection = Substitute.For<IConfigurationSection>();
		HttpResponse = Substitute.For<HttpResponse>();
	}

	public TokenService TokenService { get; }
	public IConfiguration Configuration { get; }
	public ILogger<TokenService> Logger { get; }

    public byte[] Bytes { get; }
	public string ValidToken { get; }
	public string InvalidToken { get; }
	public Player PlayerWithValidToken { get; }
    public Player PlayerWithExpiredToken { get; }
	public IConfigurationSection ConfigurationSection { get; }
	public HttpResponse HttpResponse { get; }

	private Player GetPlayerWithValidToken()
	{
		return new Player()
		{
			Id = 1,
			Name = ValidToken,
            Role = PlayerRole.Player,
            PasswordHash = Bytes,
			PasswordSalt = Bytes,
			RefreshToken = ValidToken,
			RefreshTokenExpiryDate = DateTime.UtcNow
        };
    }

    private Player GetPlayerWithExpiredToken()
    {
        return new Player()
        {
            Id = 1,
            Name = ValidToken,
            Role = PlayerRole.Player,
            PasswordHash = Bytes,
            PasswordSalt = Bytes,
            RefreshToken = ValidToken,
            RefreshTokenExpiryDate = DateTime.UtcNow.AddMinutes(-5)
        };
    }
}
