using Application.Tests.Fixtures;
using Core.Entities;
using Core.Exceptions;
using Core.Models;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;

namespace Application.Tests;

[TestFixture]
public class PlayerServiceTests
{
    private PlayerServiceFixture _fixture = default!;

    [OneTimeSetUp]
    public void SetUp()
    {
        _fixture = new PlayerServiceFixture();
    }

    [Test]
    public async Task GetAllAsync_ValidParameters_ReturnsPaginatedListOfPlayer()
    {
        // Arrange
        _fixture.PlayerRepository
            .GetAllAsync(Arg.Any<int>(), Arg.Any<int>())
            .Returns(_fixture.PaginatedList);

        // Act
        var result = await _fixture.PlayerService.GetAllAsync(_fixture.Id, _fixture.Id);

        // Assert
        result.Should().NotBeNull().And.NotBeEmpty().And.BeOfType<PaginatedList<Player>>();
    }

    [Test]
    public async Task GetByIdAsync_ExistingPlayer_ReturnsPlayer()
    {
        // Arrange
        _fixture.PlayerRepository
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Player);

        // Act
        var result = await _fixture.PlayerService.GetByIdAsync(_fixture.Id);

        // Assert
        result.Should().NotBeNull().And.BeOfType<Player>();
    }

    [Test]
    public async Task GetByIdAsync_NonexistingPlayer_ThrowsNullReferenceException()
    {
        // Arrange
        _fixture.PlayerRepository
            .GetByIdAsync(Arg.Any<int>())
            .ReturnsNull();

        // Act
        var result = async () => await _fixture.PlayerService.GetByIdAsync(_fixture.Id);

        // Assert
        await result.Should().ThrowAsync<NullReferenceException>();
    }

    [Test]
    public async Task GetByNameAsync_ExistingPlayer_ReturnsPlayer()
    {
        // Arrange
        _fixture.PlayerRepository
            .GetByNameAsync(Arg.Any<string>())
            .Returns(_fixture.Player);

        // Act
        var result = await _fixture.PlayerService.GetByNameAsync(_fixture.Name!);

        // Assert
        result.Should().NotBeNull().And.BeOfType<Player>();
    }

    [Test]
    public async Task GetByNameAsync_NonexistingPlayer_ThrowsNullReferenceException()
    {
        // Arrange
        _fixture.PlayerRepository
            .GetByNameAsync(Arg.Any<string>())
            .ReturnsNull();

        // Act
        var result = async () => await _fixture.PlayerService.GetByNameAsync(_fixture.Name!);

        // Assert
        await result.Should().ThrowAsync<NullReferenceException>();
    }

    [Test]
    public void AddAsync_ValidPlayer_ReturnsTask()
    {
        // Act
        var result = _fixture.PlayerService.AddAsync(_fixture.Player);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void UpdateAsync_ValidPlayer_ReturnsTask()
    {
        // Act
        var result = _fixture.PlayerService.UpdateAsync(_fixture.Player);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void DeleteAsync_ValidPlayer_ReturnsTask()
    {
        // Act
        var result = _fixture.PlayerService.DeleteAsync(_fixture.Player);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void CreatePlayer_ValidPlayer_ReturnsPlayer()
    {
        // Act
        var result = _fixture.PlayerService.CreatePlayer(_fixture.Name!, _fixture.Bytes, _fixture.Bytes);

        // Assert
        result.Should().NotBeNull().And.BeOfType<Player>();
    }

    [Test]
    public void ChangePasswordData_ValidData_ReturnsVoid()
    {
        // Act
        var result = () => _fixture.PlayerService
            .ChangePasswordData(_fixture.Player, _fixture.Bytes, _fixture.Bytes);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void VerifyPlayerAccessRights_SufficientRights_ReturnsVoid()
    {
        // Act
        var result = () => _fixture.PlayerService
            .VerifyPlayerAccessRights(_fixture.Player, _fixture.IIdentity, _fixture.SufficientClaims);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void VerifyPlayerAccessRights_InsufficientRights_ReturnsVoid()
    {
        // Act
        var result = () => _fixture.PlayerService
            .VerifyPlayerAccessRights(_fixture.Player, _fixture.IIdentity, _fixture.InsufficientClaims);

        // Assert
        result.Should().Throw<NotEnoughRightsException>();
    }
}
