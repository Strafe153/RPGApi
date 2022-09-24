﻿using Core.Dtos;
using Core.Dtos.PlayerDtos;
using Core.Entities;
using Core.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using WebApi.Tests.Fixtures;

namespace WebApi.Tests;

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
    public async Task GetAsync_ValidPageParameters_ReturnsActionResultOfPageDtoOfPlayerReadDto()
    {
        // Arrange
        _fixture.PlayerService
            .GetAllAsync(Arg.Any<int>(), Arg.Any<int>())
            .Returns(_fixture.PaginatedList);

        _fixture.PaginatedMapper
            .Map(Arg.Any<PaginatedList<Player>>())
            .Returns(_fixture.PageDto);

        // Act
        var result = await _fixture.PlayerContainer.GetAsync(_fixture.PageParameters);
        var objectResult = result.Result.As<OkObjectResult>();
        var pageDto = objectResult.Value.As<PageDto<PlayerReadDto>>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<ActionResult<PageDto<PlayerReadDto>>>();
        objectResult.StatusCode.Should().Be(200);
        pageDto.Entities.Should().NotBeEmpty();
    }

    [Test]
    public async Task GetAsync_ExistingPlayer_ReturnsActionResultOfPlayerReadDto()
    {
        // Arrange
        _fixture.PlayerService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Player);

        _fixture.ReadMapper
            .Map(Arg.Any<Player>())
            .Returns(_fixture.PlayerReadDto);

        // Act
        var result = await _fixture.PlayerContainer.GetAsync(_fixture.Id);
        var objectResult = result.Result.As<OkObjectResult>();
        var readDto = objectResult.Value.As<PlayerReadDto>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<ActionResult<PlayerReadDto>>();
        objectResult.StatusCode.Should().Be(200);
        readDto.Should().NotBeNull();
    }

    [Test]
    public async Task RegisterAsync_ValidDto_ReturnsActionResultOfPlayerReadDto()
    {
        // Arrange
        _fixture.ReadMapper
            .Map(Arg.Any<Player>())
            .Returns(_fixture.PlayerReadDto);

        // Act
        var result = await _fixture.PlayerContainer.RegisterAsync(_fixture.PlayerAuthorizeDto);
        var objectResult = result.Result.As<CreatedAtActionResult>();
        var readDto = objectResult.Value.As<PlayerReadDto>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<ActionResult<PlayerReadDto>>();
        objectResult.StatusCode.Should().Be(201);
        readDto.Should().NotBeNull();
    }

    [Test]
    public async Task LoginAsync_ValidDto_ReturnsActionResultOfPlayerWithTokenReadDto()
    {
        // Arrange
        _fixture.PlayerService
            .GetByNameAsync(Arg.Any<string>())
            .Returns(_fixture.Player);

        _fixture.ReadWithTokenMapper
            .Map(Arg.Any<Player>())
            .Returns(_fixture.PlayerWithTokenReadDto);

        // Act
        var result = await _fixture.PlayerContainer.LoginAsync(_fixture.PlayerAuthorizeDto);
        var objectResult = result.Result.As<OkObjectResult>();
        var readWithTokenDto = objectResult.Value.As<PlayerWithTokenReadDto>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<ActionResult<PlayerWithTokenReadDto>>();
        objectResult.StatusCode.Should().Be(200);
        readWithTokenDto.Should().NotBeNull();
    }

    [Test]
    public async Task UpdateAsync_ValidDto_ReturnsNoContentResult()
    {
        // Arrange
        _fixture.PlayerService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Player);

        // Act
        var result = await _fixture.PlayerContainer.UpdateAsync(_fixture.Id, _fixture.PlayerUpdateDto);
        var objectResult = result.As<NoContentResult>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        objectResult.StatusCode.Should().Be(204);
    }

    [Test]
    public async Task ChangePasswordAsync_ValidDto_ReturnsNoContentResult()
    {
        // Arrange
        _fixture.PlayerService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Player);

        // Act
        var result = await _fixture.PlayerContainer.ChangePasswordAsync(_fixture.Id, _fixture.PlayerChangePasswordDto);
        var objectResult = result.As<NoContentResult>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        objectResult.StatusCode.Should().Be(204);
    }

    [Test]
    public async Task ChangeRoleAsync_ExistingPlayerValidRole_ReturnsNoContentResult()
    {
        // Arrange
        _fixture.PlayerService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Player);

        // Act
        var result = await _fixture.PlayerContainer.ChangeRoleAsync(_fixture.Id, _fixture.PlayerChangeRoleDto);
        var objectResult = result.As<NoContentResult>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        objectResult.StatusCode.Should().Be(204);
    }

    [Test]
    public async Task DeleteAsync_ExistingPlayer_ReturnsNoContentResult()
    {
        // Arrange
        _fixture.PlayerService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Player);

        // Act
        var result = await _fixture.PlayerContainer.DeleteAsync(_fixture.Id);
        var objectResult = result.As<NoContentResult>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        objectResult.StatusCode.Should().Be(204);
    }
}
