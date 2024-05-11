using Application.Helpers;
using Application.Mappers.Implementations;
using Application.Services.Abstractions;
using Application.Services.Implementations;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Bogus;
using Domain.Dtos.PlayerDtos;
using Domain.Dtos.TokensDtos;
using Domain.Entities;
using Domain.Enums;
using Domain.Helpers;
using Domain.Repositories;
using Domain.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Security.Claims;

namespace Application.Tests.Fixtures;

public class PlayerServiceFixture
{
	private static readonly IFixture _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

	public PlayerServiceFixture()
	{
		var generalFaker = new Faker();

		Id = Random.Shared.Next();
		Name = generalFaker.Internet.UserName();
		AccessToken = $"{generalFaker.Random.String2(32)}.{generalFaker.Random.String2(64)}.{generalFaker.Random.String2(32)}";
		RefreshToken = generalFaker.Random.String2(64);
		PlayersCount = Random.Shared.Next(1, 20);
		PageParameters = new()
		{
			PageNumber = Random.Shared.Next(1, 25),
			PageSize = Random.Shared.Next(1, 100)
		};

		var password = generalFaker.Internet.Password();
		var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt(password);

		var playerFaker = new Faker<Player>()
			.RuleFor(p => p.Id, f => f.Random.Int())
			.RuleFor(p => p.Name, Name)
			.RuleFor(p => p.Role, f => f.PickRandom<PlayerRole>())
			.RuleFor(p => p.PasswordHash, hash)
			.RuleFor(p => p.PasswordSalt, salt)
			.RuleFor(p => p.RefreshToken, f => f.Random.String2(64))
			.RuleFor(p => p.RefreshTokenExpiryDate, f => f.Date.Future());

		var playerAuthorizeDtoFaker = new Faker<PlayerAuthorizeDto>()
			.CustomInstantiator(f => new(f.Internet.UserName(), password));

		var playerUpdateDtoFaker = new Faker<PlayerUpdateDto>()
			.CustomInstantiator(f => new(f.Internet.UserName()));

		var playerChangePasswordDtoFaker = new Faker<PlayerChangePasswordDto>()
			.CustomInstantiator(f => new(f.Internet.Password()));

		var playerChangeRoleDtoFaker = new Faker<PlayerChangeRoleDto>()
			.CustomInstantiator(f => new(f.PickRandom<PlayerRole>()));

		var tokensRefreshDtoFaker = new Faker<TokensRefreshDto>()
			.CustomInstantiator(f => new(f.Random.String2(64)));

		var pagedListFaker = new Faker<PagedList<Player>>()
			.CustomInstantiator(f => new(
				playerFaker.Generate(PlayersCount),
				PlayersCount,
				f.Random.Int(1, 2),
				f.Random.Int(1, 2)));

		PlayersRepository = _fixture.Freeze<IPlayersRepository>();
		AccessHelper = default!;
		TokenHelper = _fixture.Freeze<ITokenHelper>();
		Logger = _fixture.Freeze<ILogger<PlayersService>>();
		
		ConfigureAccessRights();

		PlayersService = new PlayersService(
			PlayersRepository,
			AccessHelper,
			TokenHelper,
			new PlayerMapper(),
			Logger);

		Player = playerFaker.Generate();
		PlayerAuthorizeDto = playerAuthorizeDtoFaker.Generate();
		PlayerUpdateDto = playerUpdateDtoFaker.Generate();
		PlayerChangePasswordDto = playerChangePasswordDtoFaker.Generate();
		PlayerChangeRoleDto = playerChangeRoleDtoFaker.Generate();
		TokensRefreshDto = tokensRefreshDtoFaker.Generate();
		PagedList = pagedListFaker.Generate();
	}

	private int PlayersCount { get; }

	public IPlayersService PlayersService { get; }
	public IPlayersRepository PlayersRepository { get; }
	public IAccessHelper AccessHelper { get; private set; }
	public ITokenHelper TokenHelper { get; }
	public ILogger<PlayersService> Logger { get; }

	public int Id { get; }
	public PageParameters PageParameters { get; }
	public string Name { get; }
	public string AccessToken { get; }
	public string RefreshToken { get; }
	public Player Player { get; }
	public PlayerAuthorizeDto PlayerAuthorizeDto { get; }
	public PlayerUpdateDto PlayerUpdateDto { get; }
	public PlayerChangePasswordDto PlayerChangePasswordDto { get; }
	public PlayerChangeRoleDto PlayerChangeRoleDto { get; }
	public TokensRefreshDto TokensRefreshDto { get; }
	public PagedList<Player> PagedList { get; }
	public CancellationToken CancellationToken { get; }

	public void ConfigureAccessRights(bool useSufficientClaims = true)
	{
		var httpContext = _fixture.Freeze<HttpContext>();

		var claims = useSufficientClaims ? SufficientClaims : InsufficientClaims;
		httpContext.User.Claims.Returns(claims);

		var httpContextAccessor = _fixture.Freeze<IHttpContextAccessor>();
		httpContextAccessor.HttpContext.Returns(httpContext);

		AccessHelper = new AccessHelper(httpContextAccessor);
	}

	private static IEnumerable<Claim> InsufficientClaims =>
		new List<Claim>
		{
			new(ClaimTypes.Name, string.Empty),
			new(ClaimTypes.Role, string.Empty)
		};

	private IEnumerable<Claim> SufficientClaims =>
		new List<Claim>
		{
			new (ClaimTypes.Name, Name),
			new (ClaimTypes.Role, nameof(PlayerRole.Player))
		};
}
