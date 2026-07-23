using Application.Dtos.PlayerDtos;
using Application.Dtos.TokenDtos;
using Application.Tests.Fixtures;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Helpers;
using Domain.Shared;
using Moq;
using Npgsql;
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
		List<Player> players = [
            new()
            {
                Id = 1,
                Name = "Ren",
                Role = PlayerRole.Player
            },
            new()
            {
                Id = 2,
                Name = "Courtney",
                Role = PlayerRole.Player
            },
        ];

        PagedList<Player> pagedList = new(players, players.Count, 1, 2);

        PageParameters pageParameters = new()
        {
            PageNumber = 1,
            PageSize = 2
        };

        _fixture.PlayersRepository
            .Setup(r => r.GetAllAsync(It.IsAny<PageParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedList);

        // Act
        var sut = _fixture.CreeateSut();
		var result = await sut.GetAllAsync(pageParameters, new CancellationToken());

		// Assert
		Assert.AreEqual("Ren", result.Entities.First().Name);
		Assert.AreEqual(2, result.Entities.Last().Id);
	}

	[Test]
	public async Task GetByIdAsync_Should_ReturnPlayerReadDto_WhenPlayerExists()
    {
        // Arrange
        var id = 1;
        var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");

        Player player = new()
        {
            Id = id,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        _fixture.PlayersRepository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(player);

        // Act
        var sut = _fixture.CreeateSut();
        var result = await sut.GetByIdAsync(1, new CancellationToken());

        // Assert
        Assert.AreEqual("Ren", result.Name);
	}

	[Test]
	public void GetByIdAsync_Should_ThrowNullReferenceException_WhenPlayerDoesNotExist()
	{
	    // Arrange
        var id = 1;
        var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");

        Player player = new()
        {
            Id = id,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        _fixture.PlayersRepository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(player);

        // Act
        var sut = _fixture.CreeateSut();
        Task<PlayerReadDto> result() => sut.GetByIdAsync(2, new CancellationToken());

        // Assert
        Assert.ThrowsAsync<NullReferenceException>(result);
    }

	[Test]
	public async Task AddAsync_Should_ReturnPlayerReadDto_WhenNameIsUnique()
    {
        // Arrange
        var id = 1;
        var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");

        Player player = new()
        {
            Id = id,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        PlayerAuthorizeDto authorizeDto = new("Ren", "1234Qwer");

        _fixture.PlayersRepository
            .Setup(r => r.AddAsync(It.IsAny<Player>()))
            .ReturnsAsync(id);

        // Act
        var sut = _fixture.CreeateSut();
        var result = await sut.AddAsync(authorizeDto);

        // Assert
        Assert.AreEqual(1, result.Id);
	}

	[Test]
	public void AddAsync_Should_ThrowNameNotUniqueException_WhenNameIsNotUnique()
    {
        // Arrange
        var id = 1;
        var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");

        Player player = new()
        {
            Id = id,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        PlayerAuthorizeDto authorizeDto = new("Ren", "1234Qwer");

        _fixture.PlayersRepository
            .Setup(r => r.AddAsync(It.IsAny<Player>()))
            .Throws<NpgsqlException>();

        // Act
        var sut = _fixture.CreeateSut();
        Task<PlayerReadDto> result() => sut.AddAsync(authorizeDto);

        // Assert
        Assert.ThrowsAsync<NameNotUniqueException>(result);
    }

	[Test]
	public async Task LoginAsync_Should_ReturnTokensReadDto_WhenPlayerExists()
    {
        // Arrange
        var name = "Ren";
        var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");

        Player player = new()
        {
            Id = 1,
            Name = name,
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        PlayerAuthorizeDto authorizeDto = new(name, "1234Qwer");

		_fixture.PlayersRepository
			.Setup(r => r.GetByNameAsync(name, It.IsAny<CancellationToken>()))
			.ReturnsAsync(player);

        // Act
        var sut = _fixture.CreeateSut();
        var result = await sut.LoginAsync(authorizeDto, new CancellationToken());

        // Assert
        Assert.IsTrue(result.AccessToken.Length > 0);
        Assert.IsTrue(result.RefreshToken.Length > 0);
	}

	[Test]
	public void LoginAsync_Should_ThrowNullReferenceException_WhenPlayerDoesNotExist()
	{
        // Arrange
        var name = "Ren";
        var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");

        Player player = new()
        {
            Id = 1,
            Name = name,
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        PlayerAuthorizeDto authorizeDto = new("Courtney", "53cr3t!");

    	_fixture.PlayersRepository
    		.Setup(r => r.GetByNameAsync(name, It.IsAny<CancellationToken>()))
    		.ReturnsAsync(player);

        // Act
        var sut = _fixture.CreeateSut();
        Task<TokensReadDto> result() => sut.LoginAsync(authorizeDto, new CancellationToken());

        // Assert
        Assert.ThrowsAsync<NullReferenceException>(result);
    }

	[Test]
	public void UpdateAsync_Should_ReturnTask_WhenPlayerExistsAndNameIsUnique()
    {
		// Arrange
		var id = 1;
        var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");

        Player player = new()
        {
            Id = id,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        PlayerUpdateDto updateDto = new("Ryuji");

        _fixture.PlayersRepository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(player);

        // Act
        var sut = _fixture.CreeateSut();
        Task result() => sut.UpdateAsync(1, updateDto, new CancellationToken());

        // Assert
        Assert.DoesNotThrowAsync(result);
    }

	[Test]
	public void UpdateAsync_Should_ThrowNameNotUniqueException_WhenPlayerExistsAndNameIsNotUnique()
    {
		// Arrange
		var id = 1;
        var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");

        Player player = new()
        {
            Id = id,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        PlayerUpdateDto updateDto = new("Ryuji");

        _fixture.PlayersRepository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(player);

        _fixture.PlayersRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Player>()))
            .Throws<NpgsqlException>();

        // Act
        var sut = _fixture.CreeateSut();
        Task result() => sut.UpdateAsync(id, updateDto, new CancellationToken());

        // Assert
        Assert.ThrowsAsync<NameNotUniqueException>(result);
    }

	[Test]
	public void UpdateAsync_Should_ThrowNullReferenceException_WhenPlayerExistsAndNameIsUnique()
    {
		// Arrange
		var id = 1;
        var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");

        Player player = new()
        {
            Id = id,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        PlayerUpdateDto updateDto = new("Ryuji");

        _fixture.PlayersRepository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(player);

        // Act
        var sut = _fixture.CreeateSut();
        Task result() => sut.UpdateAsync(2, updateDto, new CancellationToken());

        // Assert
        Assert.ThrowsAsync<NullReferenceException>(result);
    }

	[Test]
	public void UpdateAsync_Should_ThrowNotEnoughRightsException_WhenPlayerDoesNotHaveSufficientRights()
    {
		// Arrange
		var id = 1;
        var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");

        Player player = new()
        {
            Id = id,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        PlayerUpdateDto updateDto = new("Ryuji");

		_fixture.PlayersRepository
			.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
			.ReturnsAsync(player);

        _fixture.ConfigureAccessHelper(false);

        // Act
        var sut = _fixture.CreeateSut();
        Task result() => sut.UpdateAsync(1, updateDto, new CancellationToken());

        // Assert
        Assert.ThrowsAsync<NotEnoughRightsException>(result);
	}

	[Test]
	public void DeleteAsync_Should_ReturnTask_WhenPlayerExists()
    {
		// Arrange
		var id = 1;
        var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");

        Player player = new()
        {
            Id = id,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        _fixture.PlayersRepository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(player);

        // Act
        var sut = _fixture.CreeateSut();
        Task result() => sut.DeleteAsync(1, new CancellationToken());

        // Assert
        Assert.DoesNotThrowAsync(result);
    }

	[Test]
	public void DeleteAsync_Should_ThrowNullReferenceException_WhenPlayerDoesNotExist()
	{
		// Arrange
		var id = 1;
        var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");

        Player player = new()
        {
            Id = id,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        _fixture.PlayersRepository
            .Setup(r => r.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(player);

        // Act
        var sut = _fixture.CreeateSut();
		Task result() => sut.DeleteAsync(1, new CancellationToken());

        // Assert
        Assert.ThrowsAsync<NullReferenceException>(result);
	}

	[Test]
	public async Task DeleteAsync_Should_ThrowNotEnoughRightsException_WhenPlayerDoesNotHaveSufficientRights()
	{
    	// Arrange
    	var id = 1;
        var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");

        Player player = new()
        {
            Id = id,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        _fixture.PlayersRepository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(player);

        _fixture.ConfigureAccessHelper(false);

        // Act
        var sut = _fixture.CreeateSut();
        Task result() => sut.DeleteAsync(1, new CancellationToken());

        // Assert
        Assert.ThrowsAsync<NotEnoughRightsException>(result);
	}

	[Test]
	public async Task ChangePasswordAsync_Should_ReturnTokensReadDto_WhenPlayerExists()
    {
		// Arrange
		var id = 1;
        var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");

        Player player = new()
        {
            Id = id,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        PlayerChangePasswordDto dto = new("N3w#P4s$");

        _fixture.PlayersRepository
			.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
			.ReturnsAsync(player);

		// Act
        var sut = _fixture.CreeateSut();
        var result = await sut.ChangePasswordAsync(1, dto, new CancellationToken());

        // Assert
        Assert.IsTrue(result.AccessToken.Length > 0);
        Assert.IsTrue(result.RefreshToken.Length > 0);
	}

	[Test]
	public void ChangePasswordAsync_Should_ThrowNullReferenceException_WhenPlayerDoesNotExist()
	{
    	// Arrange
    	var id = 1;
        var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");

        Player player = new()
        {
            Id = id,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        PlayerChangePasswordDto dto = new("N3w#P4s$");

        _fixture.PlayersRepository
    		.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
    		.ReturnsAsync(player);

		// Act
        var sut = _fixture.CreeateSut();
        Task result() => sut.ChangePasswordAsync(2, dto, new CancellationToken());

        // Assert
        Assert.ThrowsAsync<NullReferenceException>(result);
	}

	[Test]
	public void ChangePasswordAsync_Should_ThrowNotEnoughRightsException_WhenPlayerDoesNotHaveSufficientRights()
	{
    	// Arrange
    	var id = 1;
        var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");

        Player player = new()
        {
            Id = id,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        PlayerChangePasswordDto dto = new("N3w#P4s$");

        _fixture.PlayersRepository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(player);

        _fixture.ConfigureAccessHelper(false);

		// Act
        var sut = _fixture.CreeateSut();
        Task result() => sut.ChangePasswordAsync(1, dto, new CancellationToken());

        // Assert
        Assert.ThrowsAsync<NotEnoughRightsException>(result);
	}

	[Test]
	public async Task ChangeRoleAsync_Should_ReturnPlayerReadDto_WhenPlayerExists()
    {
		// Arrange
		var id = 1;
        var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");

        Player player = new()
        {
            Id = id,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        PlayerChangeRoleDto changeRoleDto = new(PlayerRole.Admin);

        _fixture.PlayersRepository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(player);

        // Act
        var sut = _fixture.CreeateSut();
		var result = await sut.ChangeRoleAsync(1, changeRoleDto, new CancellationToken());

        // Assert
        Assert.AreEqual(PlayerRole.Admin, result.Role);
	}

	[Test]
	public void ChangeRoleAsync_Should_ThrowNullReferenceException_WhenPlayerDoesNotExist()
	{
    	// Arrange
    	var id = 1;
        var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");

        Player player = new()
        {
            Id = id,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        PlayerChangeRoleDto changeRoleDto = new(PlayerRole.Admin);

        _fixture.PlayersRepository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(player);

        // Act
        var sut = _fixture.CreeateSut();
        Task result() => sut.ChangeRoleAsync(2, changeRoleDto, new CancellationToken());

        // Assert
        Assert.ThrowsAsync<NullReferenceException>(result);
	}

	[Test]
	public async Task RefreshTokensAsync_Should_ReturnTokensReadDto_WhenPlayerExists()
	{
		// Arrange
		var id = 1;
        var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");
        var refreshToken = _fixture.TokenHelper.GenerateRefreshToken();

        Player player = new()
        {
            Id = id,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt,
            RefreshToken = refreshToken,
            RefreshTokenExpiryDate = DateTime.UtcNow.AddMonths(1)
        };

        TokensRefreshDto tokensDto = new(refreshToken);

        _fixture.PlayersRepository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(player);

        // Act
        var sut = _fixture.CreeateSut();
		var result = await sut.RefreshTokensAsync(1, tokensDto, new CancellationToken());

		// Assert
		Assert.IsTrue(result.AccessToken.Length > 0);
        Assert.IsTrue(result.RefreshToken.Length > 0);
	}

	[Test]
	public void RefreshTokensAsync_Should_ThrowNullReferenceException_WhenPlayerDoesNotExist()
	{
    	// Arrange
    	var id = 1;
        var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");
        var refreshToken = _fixture.TokenHelper.GenerateRefreshToken();

        Player player = new()
        {
            Id = id,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt,
            RefreshToken = refreshToken,
            RefreshTokenExpiryDate = DateTime.UtcNow.AddMonths(1)
        };

        TokensRefreshDto tokensDto = new(refreshToken);

        _fixture.PlayersRepository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(player);

        // Act
        var sut = _fixture.CreeateSut();
        Task result() => sut.RefreshTokensAsync(2, tokensDto, new CancellationToken());

        // Assert
        Assert.ThrowsAsync<NullReferenceException>(result);
	}

	[Test]
	public void RefreshTokensAsync_Should_ThrowNotEnoughRightsException_WhenPlayerDoesNotHaveSufficientRights()
	{
	    // Arrange
    	var id = 1;
        var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");
        var refreshToken = _fixture.TokenHelper.GenerateRefreshToken();

        Player player = new()
        {
            Id = id,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt,
            RefreshToken = refreshToken,
            RefreshTokenExpiryDate = DateTime.UtcNow.AddMonths(1)
        };

        TokensRefreshDto tokensDto = new(refreshToken);

        _fixture.PlayersRepository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(player);

        _fixture.ConfigureAccessHelper(false);

        // Act
        var sut = _fixture.CreeateSut();
        Task result() => sut.RefreshTokensAsync(1, tokensDto, new CancellationToken());

        // Assert
        Assert.ThrowsAsync<NotEnoughRightsException>(result);
	}
}
