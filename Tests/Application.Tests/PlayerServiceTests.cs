using Application.Tests.Fixtures;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Shared;
using FluentAssertions;
using Npgsql;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;

namespace Application.Tests;

[TestFixture]
public class PlayerServiceTests
{
    private PlayerServiceFixture _fixture = default!;

    [SetUp]
    public void SetUp() => _fixture = new PlayerServiceFixture();

    [Test]
    public async Task GetAllAsync_Should_ReturnPaginatedListOfPlayer_WhenParametersAreValid()
    {
        // Arrange
        _fixture.PlayerRepository
            .GetAllAsync(Arg.Any<int>(), Arg.Any<int>())
            .Returns(_fixture.PaginatedList);

        // Act
        var result = await _fixture.PlayerService.GetAllAsync(_fixture.PageNumber, _fixture.PageSize);

        // Assert
        result.Should().NotBeNull().And.NotBeEmpty().And.BeOfType<PaginatedList<Player>>();
    }

    [Test]
    public async Task GetByIdAsync_Should_ReturnPlayer_WhenPlayerExists()
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
    public async Task GetByIdAsync_Should_ThrowNullReferenceException_WhenPlayerDoesNotExist()
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
    public async Task GetByNameAsync_Should_ReturnPlayer_WhenPlayerExists()
    {
        // Arrange
        _fixture.PlayerRepository
            .GetByNameAsync(Arg.Any<string>())
            .Returns(_fixture.Player);

        // Act
        var result = await _fixture.PlayerService.GetByNameAsync(_fixture.Name);

        // Assert
        result.Should().NotBeNull().And.BeOfType<Player>();
    }

    [Test]
    public async Task GetByNameAsync_Should_ThrowNullReferenceException_WhenPlayerDoesNotExist()
    {
        // Arrange
        _fixture.PlayerRepository
            .GetByNameAsync(Arg.Any<string>())
            .ReturnsNull();

        // Act
        var result = async () => await _fixture.PlayerService.GetByNameAsync(_fixture.Name);

        // Assert
        await result.Should().ThrowAsync<NullReferenceException>();
    }

    [Test]
    public void AddAsync_Should_ReturnTask_WhenNameIsUnique()
    {
        // Act
        var result = _fixture.PlayerService.AddAsync(_fixture.Player);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public async Task AddAsync_Should_ReturnTask_WhenNameIsNotUnique()
    {
        // Arrange
        _fixture.PlayerRepository
            .AddAsync(_fixture.Player)
            .ThrowsAsync<NpgsqlException>();

        // Act
        var result = async () => await _fixture.PlayerService.AddAsync(_fixture.Player);

        // Assert
        await result.Should().ThrowAsync<NameNotUniqueException>();
    }

    [Test]
    public void UpdateAsync_Should_ReturnTask_WhenNameIsUnique()
    {
        // Act
        var result = _fixture.PlayerService.UpdateAsync(_fixture.Player);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public async Task UpdateAsync_Should_ReturnTask_WhenPlayerIsNotUnique()
    {
        // Arrange
        _fixture.PlayerRepository
            .UpdateAsync(_fixture.Player)
            .ThrowsAsync<NpgsqlException>();

        // Act
        var result = async () => await _fixture.PlayerService.UpdateAsync(_fixture.Player);

        // Assert
        await result.Should().ThrowAsync<NameNotUniqueException>();
    }

    [Test]
    public void DeleteAsync_Should_ReturnTask_WhenPlayerIsValid()
    {
        // Act
        var result = _fixture.PlayerService.DeleteAsync(_fixture.Id);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void CreatePlayer_Should_ReturnPlayer_WhenPlayerIsValid()
    {
        // Act
        var result = _fixture.PlayerService.CreatePlayer(_fixture.Name, _fixture.Bytes, _fixture.Bytes);

        // Assert
        result.Should().NotBeNull().And.BeOfType<Player>();
    }

    [Test]
    public void ChangePasswordData_Should_ReturnVoid_WhenDataIsValid()
    {
        // Act
        var result = () => _fixture.PlayerService
            .ChangePasswordData(_fixture.Player, _fixture.Bytes, _fixture.Bytes);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void VerifyPlayerAccessRights_Should_ReturnVoid_WhenClaimsAreSufficient()
    {
        // Arrange
        _fixture.HttpContextAccessor
            .HttpContext.User.Claims
            .Returns(_fixture.SufficientClaims);

        // Act
        var result = () => _fixture.PlayerService.VerifyPlayerAccessRights(_fixture.Player);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void VerifyPlayerAccessRights_Should_ReturnVoid_WhenClaimsAreInsufficient()
    {
        // Arrange
        _fixture.HttpContextAccessor
            .HttpContext.User.Claims
            .Returns(_fixture.InsufficientClaims);
            
        // Act
        var result = () => _fixture.PlayerService.VerifyPlayerAccessRights(_fixture.Player);

        // Assert
        result.Should().Throw<NotEnoughRightsException>();
    }
}
