using Core.Dtos;
using Core.Dtos.PlayerDtos;
using Core.Dtos.TokensDtos;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using WebApi.Tests.V1.Fixtures;

namespace WebApi.Tests.V1;

[TestFixture]
public class PlayersControllerTests
{
    private PlayersControllerFixture _fixture = default!;

    [OneTimeSetUp]
    public void SetUp()
    {
        _fixture = new PlayersControllerFixture();
    }

    [Test]
    public async Task GetAsync_Should_ReturnActionResultOfPageDtoOfPlayerReadDto_WhenPageParametersAreValid()
    {
        // Arrange
        _fixture.PlayerService
            .GetAllAsync(Arg.Any<int>(), Arg.Any<int>())
            .Returns(_fixture.PaginatedList);

        // Act
        var result = await _fixture.PlayersController.GetAsync(_fixture.PageParameters, _fixture.CancellationToken);
        var objectResult = result.Result.As<OkObjectResult>();
        var pageDto = objectResult.Value.As<PageDto<PlayerReadDto>>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<ActionResult<PageDto<PlayerReadDto>>>();
        objectResult.StatusCode.Should().Be(200);
        pageDto.Entities!.Count().Should().Be(_fixture.PlayersCount);
    }

    [Test]
    public async Task GetAsync_Should_ReturnActionResultOfPlayerReadDto_WhenPlayerExists()
    {
        // Arrange
        _fixture.PlayerService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Player);

        // Act
        var result = await _fixture.PlayersController.GetAsync(_fixture.Id, _fixture.CancellationToken);
        var objectResult = result.Result.As<OkObjectResult>();
        var readDto = objectResult.Value.As<PlayerReadDto>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<ActionResult<PlayerReadDto>>();
        objectResult.StatusCode.Should().Be(200);
        readDto.Should().NotBeNull();
    }

    [Test]
    public async Task RegisterAsync_Should_ReturnActionResultOfPlayerReadDto_WhenPlayerAuthorizeDtoIsValid()
    {
        // Arrange
        _fixture.PlayerService
            .CreatePlayer(
                Arg.Any<string>(),
                Arg.Any<byte[]>(),
                Arg.Any<byte[]>())
            .Returns(_fixture.Player);

        // Act
        var result = await _fixture.PlayersController.RegisterAsync(_fixture.PlayerAuthorizeDto);
        var objectResult = result.Result.As<CreatedAtActionResult>();
        var readDto = objectResult.Value.As<PlayerReadDto>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<ActionResult<PlayerReadDto>>();
        objectResult.StatusCode.Should().Be(201);
        readDto.Should().NotBeNull();
    }

    [Test]
    public async Task LoginAsync_Should_ReturnActionResultOfTokensReadDto_WhenPlayerAuthorizeDtoIsValid()
    {
        // Arrange
        _fixture.PlayerService
            .GetByNameAsync(Arg.Any<string>())
            .Returns(_fixture.Player);

        // Act
        var result = await _fixture.PlayersController.LoginAsync(_fixture.PlayerAuthorizeDto, _fixture.CancellationToken);
        var objectResult = result.Result.As<OkObjectResult>();
        var readWithTokenDto = objectResult.Value.As<TokensReadDto>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<ActionResult<TokensReadDto>>();
        objectResult.StatusCode.Should().Be(200);
        readWithTokenDto.Should().NotBeNull();
    }

    [Test]
    public async Task UpdateAsync_Should_ReturnNoContentResult_WhenPlayerExistsAndPlayerBaseDtoIsValid()
    {
        // Arrange
        _fixture.PlayerService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Player);

        // Act
        var result = await _fixture.PlayersController.UpdateAsync(_fixture.Id, _fixture.PlayerUpdateDto);
        var objectResult = result.As<NoContentResult>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        objectResult.StatusCode.Should().Be(204);
    }

    [Test]
    public async Task DeleteAsync_Should_ReturnNoContentResult_WhenPlayerExists()
    {
        // Arrange
        _fixture.PlayerService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Player);

        // Act
        var result = await _fixture.PlayersController.DeleteAsync(_fixture.Id);
        var objectResult = result.As<NoContentResult>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        objectResult.StatusCode.Should().Be(204);
    }

    [Test]
    public async Task ChangePasswordAsync_Should_ReturnOkObjectResultOfTokensReadDto_WhenPlayerExistsAndPlayerChangePasswordDtoIsValid()
    {
        // Arrange
        _fixture.PlayerService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Player);

        // Act
        var result = await _fixture.PlayersController.ChangePasswordAsync(_fixture.Id, _fixture.PlayerChangePasswordDto);
        var objectResult = result.Result.As<OkObjectResult>();
        var tokensReadDto = objectResult.Value.As<TokensReadDto>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<ActionResult<TokensReadDto>>();
        objectResult.StatusCode.Should().Be(200);
        tokensReadDto.Should().NotBeNull();
    }

    [Test]
    public async Task ChangeRoleAsync_Should_ReturnActionResultOfPlayerReadDto_WhenPlayerExistsAndPlayerChangeRoleDtoIsValid()
    {
        // Arrange
        _fixture.PlayerService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Player);

        // Act
        var result = await _fixture.PlayersController.ChangeRoleAsync(_fixture.Id, _fixture.PlayerChangeRoleDto);
        var objectResult = result.Result.As<OkObjectResult>();
        var readDto = objectResult.Value.As<PlayerReadDto>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<ActionResult<PlayerReadDto>>();
        objectResult.StatusCode.Should().Be(200);
        readDto.Should().NotBeNull();
    }

    [Test]
    public async Task RefreshTokensAsync_Should_ReturnActionResultOfTokensReadDto_WhenPlayerExistsAndTokensRefreshDtoIsValid()
    {
        // Arrange
        _fixture.PlayerService
            .GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(_fixture.Player);

        // Act
        var result = await _fixture.PlayersController.RefreshTokensAsync(_fixture.Id, _fixture.TokensRefreshDto);
        var objectResult = result.Result.As<OkObjectResult>();
        var tokensReadDto = objectResult.Value.As<TokensReadDto>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<ActionResult<TokensReadDto>>();
        objectResult.StatusCode.Should().Be(200);
        tokensReadDto.Should().NotBeNull();
    }
}
