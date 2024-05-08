using Domain.Dtos;
using Domain.Dtos.PlayerDtos;
using Domain.Dtos.TokensDtos;
using Domain.Shared;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using WebApi.Tests.V1.Fixtures;

namespace WebApi.Tests.V1;

[TestFixture]
public class PlayersControllerTests
{
	private PlayersControllerFixture _fixture = default!;

	[SetUp]
	public void SetUp() => _fixture = new();

	[Test]
	public async Task Get_Should_ReturnActionResultOfPageDtoOfPlayerReadDto_WhenPageParametersAreValid()
	{
		// Arrange
		_fixture.PlayersService
			.GetAllAsync(Arg.Any<PageParameters>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.PageDto);

		// Act
		var result = await _fixture.PlayersController.Get(_fixture.PageParameters, _fixture.CancellationToken);
		var objectResult = result.Result.As<OkObjectResult>();
		var pageDto = objectResult.Value.As<PageDto<PlayerReadDto>>();

		// Assert
		result.Should().NotBeNull().And.BeOfType<ActionResult<PageDto<PlayerReadDto>>>();
		objectResult.StatusCode.Should().Be(StatusCodes.Status200OK);
		pageDto.Entities.Count().Should().Be(_fixture.PlayersCount);
	}

	[Test]
	public async Task Get_Should_ReturnActionResultOfPlayerReadDto_WhenPlayerExists()
	{
		// Arrange
		_fixture.PlayersService
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.PlayerReadDto);

		// Act
		var result = await _fixture.PlayersController.Get(_fixture.Id, _fixture.CancellationToken);
		var objectResult = result.Result.As<OkObjectResult>();
		var readDto = objectResult.Value.As<PlayerReadDto>();

		// Assert
		result.Should().NotBeNull().And.BeOfType<ActionResult<PlayerReadDto>>();
		objectResult.StatusCode.Should().Be(StatusCodes.Status200OK);
		readDto.Should().NotBeNull();
	}

	[Test]
	public async Task Register_Should_ReturnActionResultOfPlayerReadDto_WhenPlayerAuthorizeDtoIsValid()
	{
		// Arrange
		_fixture.PlayersService
			.AddAsync(Arg.Any<PlayerAuthorizeDto>())
			.Returns(_fixture.PlayerReadDto);

		// Act
		var result = await _fixture.PlayersController.Register(_fixture.PlayerAuthorizeDto);
		var objectResult = result.Result.As<CreatedAtActionResult>();
		var readDto = objectResult.Value.As<PlayerReadDto>();

		// Assert
		result.Should().NotBeNull().And.BeOfType<ActionResult<PlayerReadDto>>();
		objectResult.StatusCode.Should().Be(StatusCodes.Status201Created);
		readDto.Should().NotBeNull();
	}

	[Test]
	public async Task Login_Should_ReturnActionResultOfTokensReadDto_WhenPlayerAuthorizeDtoIsValid()
	{
		// Arrange
		_fixture.PlayersService
			.LoginAsync(Arg.Any<PlayerAuthorizeDto>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.TokensReadDto);

		// Act
		var result = await _fixture.PlayersController.Login(_fixture.PlayerAuthorizeDto, _fixture.CancellationToken);
		var objectResult = result.Result.As<OkObjectResult>();
		var readWithTokenDto = objectResult.Value.As<TokensReadDto>();

		// Assert
		result.Should().NotBeNull().And.BeOfType<ActionResult<TokensReadDto>>();
		objectResult.StatusCode.Should().Be(StatusCodes.Status200OK);
		readWithTokenDto.Should().NotBeNull();
	}

	[Test]
	public async Task Update_Should_ReturnNoContentResult_WhenPlayerExistsAndPlayerBaseDtoIsValid()
	{
		// Arrange
		_fixture.PlayersService
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.PlayerReadDto);

		// Act
		var result = await _fixture.PlayersController.Update(_fixture.Id, _fixture.PlayerUpdateDto, _fixture.CancellationToken);
		var objectResult = result.As<NoContentResult>();

		// Assert
		result.Should().NotBeNull().And.BeOfType<NoContentResult>();
		objectResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
	}

	[Test]
	public async Task Delete_Should_ReturnNoContentResult_WhenPlayerExists()
	{
		// Arrange
		_fixture.PlayersService
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.PlayerReadDto);

		// Act
		var result = await _fixture.PlayersController.Delete(_fixture.Id, _fixture.CancellationToken);
		var objectResult = result.As<NoContentResult>();

		// Assert
		result.Should().NotBeNull().And.BeOfType<NoContentResult>();
		objectResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
	}

	[Test]
	public async Task ChangePassword_Should_ReturnActionResultOfTokensReadDto_WhenPlayerExistsAndDtoIsValid()
	{
		// Arrange
		_fixture.PlayersService
			.ChangePasswordAsync(
				Arg.Any<int>(),
				Arg.Any<PlayerChangePasswordDto>(),
				Arg.Any<CancellationToken>())
			.Returns(_fixture.TokensReadDto);

		// Act
		var result = await _fixture.PlayersController.ChangePassword(
			_fixture.Id,
			_fixture.PlayerChangePasswordDto,
			_fixture.CancellationToken);

		var objectResult = result.Result.As<OkObjectResult>();
		var tokensReadDto = objectResult.Value.As<TokensReadDto>();

		// Assert
		result.Should().NotBeNull().And.BeOfType<ActionResult<TokensReadDto>>();
		objectResult.StatusCode.Should().Be(StatusCodes.Status200OK);
		tokensReadDto.Should().NotBeNull();
	}

	[Test]
	public async Task ChangeRole_Should_ReturnActionResultOfPlayerReadDto_WhenPlayerExistsAndDtoIsValid()
	{
		// Arrange
		_fixture.PlayersService
			.ChangeRoleAsync(
				Arg.Any<int>(),
				Arg.Any<PlayerChangeRoleDto>(),
				Arg.Any<CancellationToken>())
			.Returns(_fixture.PlayerReadDto);

		// Act
		var result = await _fixture.PlayersController.ChangeRole(
			_fixture.Id,
			_fixture.PlayerChangeRoleDto,
			_fixture.CancellationToken);

		var objectResult = result.Result.As<OkObjectResult>();
		var readDto = objectResult.Value.As<PlayerReadDto>();

		// Assert
		result.Should().NotBeNull().And.BeOfType<ActionResult<PlayerReadDto>>();
		objectResult.StatusCode.Should().Be(StatusCodes.Status200OK);
		readDto.Should().NotBeNull();
	}

	[Test]
	public async Task RefreshTokens_Should_ReturnActionResultOfTokensReadDto_WhenPlayerExistsAndTokensRefreshDtoIsValid()
	{
		// Arrange
		_fixture.PlayersService
			.RefreshTokensAsync(
				Arg.Any<int>(),
				Arg.Any<TokensRefreshDto>(),
				Arg.Any<CancellationToken>())
			.Returns(_fixture.TokensReadDto);

		// Act
		var result = await _fixture.PlayersController.RefreshTokens(
			_fixture.Id,
			_fixture.TokensRefreshDto,
			_fixture.CancellationToken);

		var objectResult = result.Result.As<OkObjectResult>();
		var tokensReadDto = objectResult.Value.As<TokensReadDto>();

		// Assert
		result.Should().NotBeNull().And.BeOfType<ActionResult<TokensReadDto>>();
		objectResult.StatusCode.Should().Be(StatusCodes.Status200OK);
		tokensReadDto.Should().NotBeNull();
	}
}
