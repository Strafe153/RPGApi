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
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Application.Tests.Fixtures;

public class PlayerServiceFixture
{
	public PlayerServiceFixture()
	{
		var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

		var generalFaker = new Faker();

		Id = Random.Shared.Next();
		Name = generalFaker.Internet.UserName();
		Bytes = generalFaker.Random.Bytes(32);
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

		PlayersRepository = fixture.Freeze<IPlayersRepository>();
		AccessHelper = fixture.Freeze<IAccessHelper>();
		TokenHelper = fixture.Freeze<ITokenHelper>();
		Logger = fixture.Freeze<ILogger<PlayersService>>();

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
		Players = playerFaker.Generate(PlayersCount);
		PagedList = pagedListFaker.Generate();
		SufficientClaims = GetSufficientClaims();
		InsufficientClaims = GetInsufficientClaims();
	}

	private int PlayersCount { get; }

	public IPlayersService PlayersService { get; }
	public IPlayersRepository PlayersRepository { get; }
	public IAccessHelper AccessHelper { get; }
	public ITokenHelper TokenHelper { get; }
	public ILogger<PlayersService> Logger { get; }

	public int Id { get; }
	public PageParameters PageParameters { get; }
	public string Name { get; }
	public byte[] Bytes { get; }
    public string AccessToken { get; }
	public string RefreshToken { get; }
	public Player Player { get; }
	public PlayerAuthorizeDto PlayerAuthorizeDto { get; }
	public PlayerUpdateDto PlayerUpdateDto { get; }
    public PlayerChangePasswordDto PlayerChangePasswordDto { get; }
	public PlayerChangeRoleDto PlayerChangeRoleDto { get; }
	public TokensRefreshDto TokensRefreshDto { get; }
	public List<Player> Players { get; }
	public PagedList<Player> PagedList { get; }
	public CancellationToken CancellationToken { get; }
	public IEnumerable<Claim> SufficientClaims { get; }
	public IEnumerable<Claim> InsufficientClaims { get; }

	private IEnumerable<Claim> GetInsufficientClaims() =>
		new List<Claim>
		{
			new(ClaimTypes.Name, string.Empty),
			new(ClaimTypes.Role, string.Empty)
		};

	private IEnumerable<Claim> GetSufficientClaims() =>
		new List<Claim>
		{
			new(ClaimTypes.Name, Name),
			new(ClaimTypes.Role, PlayerRole.Admin.ToString())
		};
}
