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
	public void SetUp() => _fixture = new();

	[Test]
	public async Task GetAllAsync_Should_ReturnPageDtoOfPlayerReadDto_WhenDataIsValid()
	{
		// Arrange
		_fixture.PlayersRepository
			.GetAllAsync(Arg.Any<PageParameters>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.PagedList);

		// Act
		var result = await _fixture.PlayersService.GetAllAsync(_fixture.PageParameters, _fixture.CancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Entities.Should().NotBeEmpty();
	}

	[Test]
	public async Task GetByIdAsync_Should_ReturnPlayerReadDto_WhenPlayerExists()
	{
		// Arrange
		_fixture.PlayersRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.Player);

		// Act
		var result = await _fixture.PlayersService.GetByIdAsync(_fixture.Id, _fixture.CancellationToken);

		// Assert
		result.Should().NotBeNull();
	}

	[Test]
	public async Task GetByIdAsync_Should_ThrowNullReferenceException_WhenPlayerDoesNotExist()
	{
		// Arrange
		_fixture.PlayersRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.ReturnsNull();

		// Act
		var result = () => _fixture.PlayersService.GetByIdAsync(_fixture.Id, _fixture.CancellationToken);

		// Assert
		await result.Should().ThrowAsync<NullReferenceException>();
	}

	[Test]
	public void AddAsync_Should_ReturnPlayerReadDto_WhenNameIsUnique()
	{
		// Arrange
		_fixture.PlayersRepository
			.AddAsync(_fixture.Player)
			.Returns(_fixture.Id);

		// Act
		var result = _fixture.PlayersService.AddAsync(_fixture.PlayerAuthorizeDto);

		// Assert
		result.Should().NotBeNull();
	}

	[Test]
	public async Task AddAsync_Should_ThrowNameNotUniqueException_WhenNameIsNotUnique()
	{
		// Arrange
		_fixture.PlayersRepository
			.AddAsync(Arg.Any<Player>())
			.ThrowsAsync<NpgsqlException>();

		// Act
		var result = () => _fixture.PlayersService.AddAsync(_fixture.PlayerAuthorizeDto);

		// Assert
		await result.Should().ThrowAsync<NameNotUniqueException>();
	}

	[Test]
	public async Task LoginAsync_Should_ReturnTokensReadDto_WhenPlayerExists()
	{
		// Arrange
		_fixture.PlayersRepository
			.GetByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.Player);

		_fixture.TokenHelper
			.GenerateAccessToken(Arg.Any<Player>())
			.Returns(_fixture.AccessToken);

		_fixture.TokenHelper
			.GenerateRefreshToken()
			.Returns(_fixture.RefreshToken);

		// Act
		var result = await _fixture.PlayersService.LoginAsync(_fixture.PlayerAuthorizeDto, _fixture.CancellationToken);

		// Assert
		result.AccessToken.Length.Should().BeGreaterThan(0);
		result.RefreshToken.Length.Should().BeGreaterThan(0);
	}

	[Test]
	public async Task LoginAsync_Should_ThrowNullReferenceException_WhenPlayerExists()
	{
		// Arrange
		_fixture.PlayersRepository
			.GetByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
			.ReturnsNull();

		// Act
		var result = () => _fixture.PlayersService.LoginAsync(_fixture.PlayerAuthorizeDto, _fixture.CancellationToken);

		// Assert
		await result.Should().ThrowAsync<NullReferenceException>();
	}

	[Test]
	public void UpdateAsync_Should_ReturnTask_WhenPlayerExistsAndNameIsUnique()
	{
		// Arrange
		_fixture.PlayersRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.Player);

		// Act
		var result = _fixture.PlayersService.UpdateAsync(_fixture.Id, _fixture.PlayerUpdateDto, _fixture.CancellationToken);

		// Assert
		result.Should().NotBeNull();
	}

	[Test]
	public async Task UpdateAsync_Should_ThrowNameNotUniqueException_WhenPlayerExistsAndNameIsNotUnique()
	{
		// Arrange
		_fixture.PlayersRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.Player);

		_fixture.PlayersRepository
			.UpdateAsync(_fixture.Player)
			.ThrowsAsync<NpgsqlException>();

		// Act
		var result = () => _fixture.PlayersService.UpdateAsync(_fixture.Id, _fixture.PlayerUpdateDto, _fixture.CancellationToken);

		// Assert
		await result.Should().ThrowAsync<NameNotUniqueException>();
	}

	[Test]
	public async Task UpdateAsync_Should_ThrowNullReferenceException_WhenPlayerExistsAndNameIsUnique()
	{
		// Arrange
		_fixture.PlayersRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.ReturnsNull();

		// Act
		var result = () => _fixture.PlayersService.UpdateAsync(
			_fixture.Id,
			_fixture.PlayerUpdateDto,
			_fixture.CancellationToken);

		// Assert
		await result.Should().ThrowAsync<NullReferenceException>();
	}

	[Test]
	public void DeleteAsync_Should_ReturnTask_WhenPlayerExists()
	{
		// Arrange
		_fixture.PlayersRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.Player);

		// Act
		var result = _fixture.PlayersService.DeleteAsync(_fixture.Id, _fixture.CancellationToken);

		// Assert
		result.Should().NotBeNull();
	}

	[Test]
	public async Task DeleteAsync_Should_ThrowNullReferenceException_WhenPlayerDoesNotExist()
	{
		// Arrange
		_fixture.PlayersRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.ReturnsNull();

		// Act
		var result = () => _fixture.PlayersService.DeleteAsync(_fixture.Id, _fixture.CancellationToken);

		// Assert
		await result.Should().ThrowAsync<NullReferenceException>();
	}

	[Test]
	public async Task ChangePasswordAsync_Should_ReturnTokensReadDto_WhenPlayerExists()
	{
		// Arrange
		_fixture.PlayersRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.Player);

		_fixture.TokenHelper
			.GenerateAccessToken(Arg.Any<Player>())
			.Returns(_fixture.AccessToken);

		_fixture.TokenHelper
			.GenerateRefreshToken()
			.Returns(_fixture.RefreshToken);

		// Act
		var result = await _fixture.PlayersService.ChangePasswordAsync(
			_fixture.Id,
			_fixture.PlayerChangePasswordDto,
			_fixture.CancellationToken);

		// Assert
		result.AccessToken.Length.Should().BeGreaterThan(0);
		result.RefreshToken.Length.Should().BeGreaterThan(0);
	}

	[Test]
	public async Task ChangePasswordAsync_Should_ThrowNullReferenceException_WhenPlayerDoesNotExist()
	{
		// Arrange
		_fixture.PlayersRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.ReturnsNull();

		// Act
		var result = () => _fixture.PlayersService.ChangePasswordAsync(
			_fixture.Id,
			_fixture.PlayerChangePasswordDto,
			_fixture.CancellationToken);

		// Assert
		await result.Should().ThrowAsync<NullReferenceException>();
	}

	[Test]
	public async Task ChangeRoleAsync_Should_ReturnPlayerReadDto_WhenPlayerExists()
	{
		// Arrange
		_fixture.PlayersRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.Player);

		// Act
		var result = await _fixture.PlayersService.ChangeRoleAsync(
			_fixture.Id,
			_fixture.PlayerChangeRoleDto,
			_fixture.CancellationToken);

		// Assert
		result.Should().NotBeNull();
	}

	[Test]
	public async Task ChangeRoleAsync_Should_ThrowNullReferenceException_WhenPlayerDoesNotExist()
	{
		// Arrange
		_fixture.PlayersRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.ReturnsNull();

		// Act
		var result = () => _fixture.PlayersService.ChangeRoleAsync(
			_fixture.Id,
			_fixture.PlayerChangeRoleDto,
			_fixture.CancellationToken);

		// Assert
		await result.Should().ThrowAsync<NullReferenceException>();
	}

	[Test]
	public async Task RefreshTokensAsync_Should_ReturnTokensReadDto_WhenPlayerExists()
	{
		// Arrange
		_fixture.PlayersRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.Player);

		_fixture.TokenHelper
			.GenerateAccessToken(Arg.Any<Player>())
			.Returns(_fixture.AccessToken);

		_fixture.TokenHelper
			.GenerateRefreshToken()
			.Returns(_fixture.RefreshToken);

		// Act
		var result = await _fixture.PlayersService.RefreshTokensAsync(
			_fixture.Id,
			_fixture.TokensRefreshDto,
			_fixture.CancellationToken);

		// Assert
		result.AccessToken.Length.Should().BeGreaterThan(0);
		result.RefreshToken.Length.Should().BeGreaterThan(0);
	}

	[Test]
	public async Task RefreshTokensAsync_Should_ThrowNullReferenceException_WhenPlayerDoesNotExist()
	{
		// Arrange
		_fixture.PlayersRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.ReturnsNull();

		// Act
		var result = () => _fixture.PlayersService.RefreshTokensAsync(
			_fixture.Id,
			_fixture.TokensRefreshDto,
			_fixture.CancellationToken);

		// Assert
		await result.Should().ThrowAsync<NullReferenceException>();
	}
}
