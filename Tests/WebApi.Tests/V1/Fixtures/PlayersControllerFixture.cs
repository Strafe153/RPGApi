using Application.Dtos;
using Application.Dtos.PlayerDtos;
using Application.Dtos.TokenDtos;
using Application.Services.Abstractions;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Bogus;
using Domain.Entities;
using Domain.Enums;
using Domain.Shared;
using WebApi.Controllers.V1;

namespace WebApi.Tests.V1.Fixtures;

public class PlayersControllerFixture
{
	public PlayersControllerFixture()
	{
		var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

		Id = Random.Shared.Next();
		Name = new Faker().Internet.UserName();
		PlayersCount = Random.Shared.Next(1, 20);

		var playerReadDtoFaker = new Faker<PlayerReadDto>()
			.CustomInstantiator(f => new(
				Id,
				f.Internet.UserName(),
				f.PickRandom<PlayerRole>(),
				new List<Character>()));

		var playerAuthorizeDtoFaker = new Faker<PlayerAuthorizeDto>()
			.CustomInstantiator(f => new(
				f.Internet.UserName(),
				f.Internet.Password()));

		var playerUpdateDtoFaker = new Faker<PlayerUpdateDto>()
			.CustomInstantiator(f => new(f.Internet.UserName()));

		var playerChangePasswordDtoFaker = new Faker<PlayerChangePasswordDto>()
			.CustomInstantiator(f => new(f.Random.String2(16)));

		var playerChangeRoleDtoFaker = new Faker<PlayerChangeRoleDto>()
			.CustomInstantiator(f => new(f.PickRandom<PlayerRole>()));

		var tokensRefreshDtoFaker = new Faker<TokensRefreshDto>()
			.CustomInstantiator(f => new(f.Random.String2(64)));

		var tokensReadDtoFaker = new Faker<TokensReadDto>()
			.CustomInstantiator(f => new(f.Random.String2(64), f.Random.String2(64)));

		var pageParametersFaker = new Faker<PageParameters>()
			.RuleFor(p => p.PageNumber, f => f.Random.Int(1, 100))
			.RuleFor(p => p.PageSize, f => f.Random.Int(1, 100));

		var pageDtoFaker = new Faker<PageDto<PlayerReadDto>>()
			.CustomInstantiator(f => new(
				1,
				f.Random.Int(1, 2),
				PlayersCount,
				PlayersCount,
				false,
				false,
				playerReadDtoFaker.Generate(PlayersCount)));

		PlayersService = fixture.Freeze<IPlayersService>();

		PlayersController = new(PlayersService);

		PlayerReadDto = playerReadDtoFaker.Generate();
		PlayerAuthorizeDto = playerAuthorizeDtoFaker.Generate();
		PlayerUpdateDto = playerUpdateDtoFaker.Generate();
		PlayerChangePasswordDto = playerChangePasswordDtoFaker.Generate();
		PlayerChangeRoleDto = playerChangeRoleDtoFaker.Generate();
		TokensRefreshDto = tokensRefreshDtoFaker.Generate();
		TokensReadDto = tokensReadDtoFaker.Generate();
		PageParameters = pageParametersFaker.Generate();
		PageDto = pageDtoFaker.Generate();
	}

	public PlayersController PlayersController { get; }
	public IPlayersService PlayersService { get; }

	public int Id { get; }
	public int PlayersCount { get; }
	public string Name { get; }
	public PlayerReadDto PlayerReadDto { get; }
	public TokensRefreshDto TokensRefreshDto { get; }
	public TokensReadDto TokensReadDto { get; }
	public PlayerAuthorizeDto PlayerAuthorizeDto { get; }
	public PlayerUpdateDto PlayerUpdateDto { get; }
	public PlayerChangePasswordDto PlayerChangePasswordDto { get; }
	public PlayerChangeRoleDto PlayerChangeRoleDto { get; }
	public PageParameters PageParameters { get; }
	public PageDto<PlayerReadDto> PageDto { get; }
	public CancellationToken CancellationToken { get; }
}
