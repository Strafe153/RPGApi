using Application.Dtos;
using Application.Dtos.PlayerDtos;
using Application.Dtos.TokenDtos;
using Domain.Entities;
using Domain.Enums;
using Domain.Helpers;
using Domain.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
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
		List<PlayerReadDto> players = [
            new(1, "Ren", PlayerRole.Player, []),
            new(2, "Courtney", PlayerRole.Player, [])
        ];

        PageDto<PlayerReadDto> pageDto = new(1, 2, 2, 2, false, false, players);

        PageParameters pageParameters = new()
        {
            PageNumber = 1,
            PageSize = 2
        };

        _fixture.PlayersService
            .Setup(r => r.GetAllAsync(It.IsAny<PageParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pageDto);

        // Act
        var sut = _fixture.CreateSut();
		var result = await sut.Get(pageParameters, new CancellationToken());
		var objectResult = result.Result as OkObjectResult;
		var resultPageDto = objectResult?.Value as PageDto<PlayerReadDto>;

		// Assert
		Assert.AreEqual(StatusCodes.Status200OK, objectResult?.StatusCode);
        Assert.AreEqual(PlayerRole.Player, resultPageDto?.Entities.First().Role);
        Assert.AreEqual("Courtney", resultPageDto?.Entities.Last().Name);
	}

	[Test]
	public async Task Get_Should_ReturnActionResultOfPlayerReadDto_WhenPlayerExists()
    {
        // Arrange
        var id = 1;
        PlayerReadDto player = new(id, "Ren", PlayerRole.Player, []);

        _fixture.PlayersService
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(player);

        // Act
        var sut = _fixture.CreateSut();
		var result = await sut.Get(1, new CancellationToken());
		var objectResult = result.Result as OkObjectResult;
		var readDto = objectResult?.Value as PlayerReadDto;

		// Assert
		Assert.AreEqual(StatusCodes.Status200OK, objectResult?.StatusCode);
        Assert.AreEqual("Ren", readDto?.Name);
	}

	[Test]
	public async Task Register_Should_ReturnActionResultOfPlayerReadDto_WhenPlayerAuthorizeDtoIsValid()
    {
        // Arrange
        PlayerReadDto player = new(1, "Ren", PlayerRole.Player, []);
        PlayerAuthorizeDto registerDto = new("Ren", "1234Qwer");

        _fixture.PlayersService
            .Setup(r => r.AddAsync(It.IsAny<PlayerAuthorizeDto>()))
            .ReturnsAsync(player);

        // Act
        var sut = _fixture.CreateSut();
		var result = await sut.Register(registerDto);
		var objectResult = result.Result as CreatedAtActionResult;
		var readDto = objectResult?.Value as PlayerReadDto;

		// Assert
		Assert.AreEqual(StatusCodes.Status201Created, objectResult?.StatusCode);
        Assert.AreEqual("Ren", readDto?.Name);
	}

	[Test]
	public async Task Login_Should_ReturnActionResultOfTokensReadDto_WhenPlayerAuthorizeDtoIsValid()
    {
        // Arrange
        var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");
        var newRefreshToken = _fixture.TokenHelper.GenerateRefreshToken();

        Player player = new()
        {
            Id = 1,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt,
            RefreshToken = newRefreshToken,
            RefreshTokenExpiryDate = DateTime.UtcNow.AddMonths(1)
        };

        var accessToken = _fixture.TokenHelper.GenerateAccessToken(player);
        var refreshToken = _fixture.TokenHelper.GenerateRefreshToken();

		TokensReadDto tokensDto = new(accessToken, refreshToken);
        PlayerAuthorizeDto authorizeDto = new("Ren", "1234Qwer");

        _fixture.PlayersService
            .Setup(r => r.LoginAsync(authorizeDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokensDto);

        // Act
        var sut = _fixture.CreateSut();
		var result = await sut.Login(authorizeDto, new CancellationToken());
		var objectResult = result.Result as OkObjectResult;
		var resultDto = objectResult?.Value as TokensReadDto;

		// Assert
		Assert.AreEqual(StatusCodes.Status200OK, objectResult?.StatusCode);
        Assert.IsTrue(resultDto?.AccessToken.Length > 0);
        Assert.IsTrue(resultDto?.RefreshToken.Length > 0);
	}

	[Test]
	public async Task Update_Should_ReturnNoContentResult_WhenPlayerExistsAndPlayerBaseDtoIsValid()
    {
		// Arrange
        PlayerUpdateDto updateDto = new("Eltolinde");

        // Act
        var sut = _fixture.CreateSut();
		var result = await sut.Update(1, updateDto, new CancellationToken());
		var objectResult = result as NoContentResult;

		// Assert
		Assert.AreEqual(StatusCodes.Status204NoContent, objectResult?.StatusCode);
	}

	[Test]
	public async Task Delete_Should_ReturnNoContentResult_WhenPlayerExists()
    {
        // Act
        var sut = _fixture.CreateSut();
		var result = await sut.Delete(1, new CancellationToken());
		var objectResult = result as NoContentResult;

		// Assert
		Assert.AreEqual(StatusCodes.Status204NoContent, objectResult?.StatusCode);
	}

	[Test]
	public async Task ChangePassword_Should_ReturnActionResultOfTokensReadDto_WhenPlayerExistsAndDtoIsValid()
    {
        // Arrange
        var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");
        var newRefreshToken = _fixture.TokenHelper.GenerateRefreshToken();

        Player player = new()
        {
            Id = 1,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt,
            RefreshToken = newRefreshToken,
            RefreshTokenExpiryDate = DateTime.UtcNow.AddMonths(1)
        };

        var accessToken = _fixture.TokenHelper.GenerateAccessToken(player);
        var refreshToken = _fixture.TokenHelper.GenerateRefreshToken();

        TokensReadDto tokensDto = new(accessToken, refreshToken);
        PlayerChangePasswordDto changePasswordDto = new("53cr3t!");

        _fixture.PlayersService
            .Setup(r => r.ChangePasswordAsync(
                It.IsAny<int>(),
                It.IsAny<PlayerChangePasswordDto>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokensDto);

        // Act
        var sut = _fixture.CreateSut();
		var result = await sut.ChangePassword(1, changePasswordDto, new CancellationToken());

		var objectResult = result.Result as OkObjectResult;
		var resultDto = objectResult?.Value as TokensReadDto;

		// Assert
		Assert.AreEqual(StatusCodes.Status200OK, objectResult?.StatusCode);
        Assert.IsTrue(resultDto?.AccessToken.Length > 0);
        Assert.IsTrue(resultDto?.RefreshToken.Length > 0);
	}

	[Test]
	public async Task ChangeRole_Should_ReturnActionResultOfPlayerReadDto_WhenPlayerExistsAndDtoIsValid()
	{
		// Arrange
		var id = 1;
        PlayerReadDto player = new(id, "Ren", PlayerRole.Admin, []);
        PlayerChangeRoleDto changeRoleDto = new(PlayerRole.Admin);

        _fixture.PlayersService
            .Setup(r => r.ChangeRoleAsync(
                id,
                It.IsAny<PlayerChangeRoleDto>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(player);

        // Act
        var sut = _fixture.CreateSut();
		var result = await sut.ChangeRole(1, changeRoleDto, new CancellationToken());

		var objectResult = result.Result as OkObjectResult;
		var readDto = objectResult?.Value as PlayerReadDto;

        // Assert
        Assert.AreEqual(StatusCodes.Status200OK, objectResult?.StatusCode);
        Assert.AreEqual(PlayerRole.Admin, readDto?.Role);
	}

	[Test]
	public async Task RefreshTokens_Should_ReturnActionResultOfTokensReadDto_WhenPlayerExistsAndTokensRefreshDtoIsValid()
    {
        // Arrange
        var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");
        var newRefreshToken = _fixture.TokenHelper.GenerateRefreshToken();

        Player player = new()
        {
            Id = 1,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt,
            RefreshToken = newRefreshToken,
            RefreshTokenExpiryDate = DateTime.UtcNow.AddMonths(1)
        };

        var accessToken = _fixture.TokenHelper.GenerateAccessToken(player);
        var refreshToken = _fixture.TokenHelper.GenerateRefreshToken();

        TokensReadDto tokensDto = new(accessToken, refreshToken);
        TokensRefreshDto refreshTokensDto = new(refreshToken);

        _fixture.PlayersService
            .Setup(r => r.RefreshTokensAsync(
                1,
                It.IsAny<TokensRefreshDto>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokensDto);

        // Act
        var sut = _fixture.CreateSut();
		var result = await sut.RefreshTokens(1, refreshTokensDto, new CancellationToken());

		var objectResult = result.Result as OkObjectResult;
		var resultDto = objectResult?.Value as TokensReadDto;

		// Assert
		Assert.AreEqual(StatusCodes.Status200OK, objectResult?.StatusCode);
        Assert.IsTrue(resultDto?.AccessToken.Length > 0);
        Assert.IsTrue(resultDto?.RefreshToken.Length > 0);
        Assert.AreNotEqual(refreshToken, newRefreshToken);
	}
}
