using Application.Dtos;
using Application.Dtos.CharactersDtos;
using Application.Tests.Fixtures;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Helpers;
using Domain.Shared;
using Microsoft.AspNetCore.JsonPatch;
using Moq;
using NUnit.Framework;

namespace Application.Tests;

[TestFixture]
public class CharacterServiceTests
{
	private CharacterServiceFixture _fixture = default!;

	[SetUp]
	public void SetUp() => _fixture = new();

	[Test]
	public async Task GetAllAsync_Should_ReturnPageDtoOfCharacterReadDto_WhenDataIsValid()
    {
        // Arrange
        List<Character> characters = [
            new()
            {
                Id = 1,
                Name = "Ren",
                Race = CharacterRace.Human,
                Health = 100,
                PlayerId = 1,
            },
            new()
            {
                Id = 2,
                Name = "Eltolinde",
                Race = CharacterRace.Elf,
                Health = 100,
                PlayerId = 1,
            },
        ];

        PagedList<Character> pagedList = new(characters, characters.Count, 1, 2);

        PageParameters pageParameters = new()
        {
            PageNumber = 1,
            PageSize = 2
        };

        _fixture.CharactersRepository
            .Setup(x => x.GetAllAsync(It.IsAny<PageParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedList);

        // Act
        var sut = _fixture.CreeateSut();
        var result = await sut.GetAllAsync(pageParameters, new CancellationToken());

        // Assert
        Assert.AreEqual(CharacterRace.Human, result.Entities.First().Race);
        Assert.AreEqual("Eltolinde", result.Entities.Last().Name);
	}

	[Test]
	public async Task GetByIdAsync_Should_ReturnCharacterReadDto_WhenCharacterExists()
    {
        // Arrange
        var id = 1;

        Character character = new()
        {
            Id = id,
            Name = "Ren",
            Race = CharacterRace.Human,
            Health = 100,
            PlayerId = 1,
        };

        _fixture.CharactersRepository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(character);

        // Act
        var sut = _fixture.CreeateSut();
		var result = await sut.GetByIdAsync(id, new CancellationToken());

        // Assert
        Assert.AreEqual("Ren", result.Name);
	}

	[Test]
	public void GetByIdAsync_Should_ThrowNullReferenceException_WhenCharacterDoesNotExist()
    {
        // Arrange
        var id = 1;

        Character character = new()
        {
            Id = id,
            Name = "Ren",
            Race = CharacterRace.Human,
            Health = 100,
            PlayerId = 1,
        };

        _fixture.CharactersRepository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(character);

        // Act
        var sut = _fixture.CreeateSut();
        Task<CharacterReadDto> result() => sut.GetByIdAsync(2, new CancellationToken());

        // Assert
        Assert.ThrowsAsync<NullReferenceException>(result);
	}

	[Test]
	public async Task AddAsync_Should_ReturnCharacterReadDto_WhenPlayerExists()
    {
        // Arrange
        var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");

        Player player = new()
        {
            Id = 1,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        CharacterCreateDto creteDto = new("Ren", 1, CharacterRace.Human);

        _fixture.PlayersRepository
            .Setup(r => r.GetByIdAsync(1, new CancellationToken()))
            .ReturnsAsync(player);

        // Act
        var sut = _fixture.CreeateSut();
        var result = await sut.AddAsync(creteDto, new CancellationToken());

        // Assert
        Assert.IsNotNull(result.Id);
        Assert.AreEqual("Ren", result.Name);
	}

	[Test]
	public void AddAsync_Should_ThrowNullReferenceException_WhenPlayerDoesNotExist()
	{
	    // Arrange
        var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");

        Player player = new()
        {
            Id = 1,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        CharacterCreateDto createDto = new("Ren", 1, CharacterRace.Human);

        _fixture.PlayersRepository
            .Setup(r => r.GetByIdAsync(2, new CancellationToken()))
            .ReturnsAsync(player);

        // Act
        var sut = _fixture.CreeateSut();
        Task<CharacterReadDto> result() => sut.AddAsync(createDto, new CancellationToken());

        // Assert
        Assert.ThrowsAsync<NullReferenceException>(result);
	}

	[Test]
	public void AddAsync_Should_ThrowNotEnoughRightsException_WhenPlayerDoesNotHaveSufficientRights()
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

        CharacterCreateDto createDto = new("Ren", 1, CharacterRace.Human);

		_fixture.PlayersRepository
			.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
			.ReturnsAsync(player);

        _fixture.ConfigureAccessHelper(false);

        // Act
        var sut = _fixture.CreeateSut();
        Task<CharacterReadDto> result() => sut.AddAsync(createDto, new CancellationToken());

        // Assert
        Assert.ThrowsAsync<NotEnoughRightsException>(result);
	}

	[Test]
	public void UpdateAsync_Should_ReturnTask_WhenCharacterExists()
    {
        // Arrange
        var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");

        Player player = new()
        {
            Id = 1,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        var id = 1;

        Character character = new()
        {
            Id = id,
            Name = "Ren",
            Race = CharacterRace.Human,
            Health = 100,
            PlayerId = 1,
            Player = player
        };

        CharacterUpdateDto updateDto = new("Eltolinde", CharacterRace.Elf);

        _fixture.CharactersRepository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(character);

        // Act
        var sut = _fixture.CreeateSut();
        Task result() => sut.UpdateAsync(1, updateDto, new CancellationToken());

        // Assert
        Assert.DoesNotThrowAsync(result);
	}

	[Test]
	public void UpdateAsync_Should_ThrowNullReferenceExceition_WhenCharacterDoesNotExist()
	{
	    // Arrange
        var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");

        Player player = new()
        {
            Id = 1,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        var id = 1;

        Character character = new()
        {
            Id = id,
            Name = "Ren",
            Race = CharacterRace.Human,
            Health = 100,
            PlayerId = 1,
            Player = player
        };

        CharacterUpdateDto updateDto = new("Eltolinde", CharacterRace.Elf);

        _fixture.CharactersRepository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(character);

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
        var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");

        Player player = new()
        {
            Id = 1,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        var id = 1;

        Character character = new()
        {
            Id = id,
            Name = "Ren",
            Race = CharacterRace.Human,
            Health = 100,
            PlayerId = 1,
            Player = player
        };

        CharacterUpdateDto updateDto = new("Eltolinde", CharacterRace.Elf);

        _fixture.CharactersRepository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(character);

        _fixture.ConfigureAccessHelper(false);

        // Act
        var sut = _fixture.CreeateSut();
        Task result() => sut.UpdateAsync(id, updateDto, new CancellationToken());

        // Assert
        Assert.ThrowsAsync<NotEnoughRightsException>(result);
	}

	[Test]
	public void DeleteAsync_Should_ReturnTask_WhenCharacterExists()
    {
        // Arrange
        var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");

        Player player = new()
        {
            Id = 1,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt
        };

		var id = 1;

        Character character = new()
        {
            Id = id,
            Name = "Ren",
            Race = CharacterRace.Human,
            Health = 100,
            PlayerId = 1,
            Player = player
        };

        _fixture.CharactersRepository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(character);

        // Act
        var sut = _fixture.CreeateSut();
        Task result() => sut.DeleteAsync(id, new CancellationToken());

        // Assert
        Assert.DoesNotThrowAsync(result);
	}

	[Test]
	public void DeleteAsync_Should_ThrowNullReferenceException_WhenCharacterDoesNotExist()
	{
	    // Arrange
        var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");

        Player player = new()
        {
            Id = 1,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        var id = 1;

        Character character = new()
        {
            Id = id,
            Name = "Ren",
            Race = CharacterRace.Human,
            Health = 100,
            PlayerId = 1,
            Player = player
        };

        _fixture.CharactersRepository
            .Setup(r => r.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(character);

        // Act
        var sut = _fixture.CreeateSut();
        Task result() => sut.DeleteAsync(id, new CancellationToken());

        // Assert
        Assert.ThrowsAsync<NullReferenceException>(result);
	}

	[Test]
	public void DeleteAsync_Should_ThrowNotEnoughRightsException_WhenPlayerDoesNotHaveSufficientRights()
	{
	    // Arrange
        var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");

        Player player = new()
        {
            Id = 1,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        var id = 1;

        Character character = new()
        {
            Id = id,
            Name = "Ren",
            Race = CharacterRace.Human,
            Health = 100,
            PlayerId = 1,
            Player = player
        };

        _fixture.CharactersRepository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(character);

        _fixture.ConfigureAccessHelper(false);

        // Act
        var sut = _fixture.CreeateSut();
        Task result() => sut.DeleteAsync(id, new CancellationToken());

        // Assert
        Assert.ThrowsAsync<NotEnoughRightsException>(result);
	}

	[Test]
	public async Task PatchAsync_Should_ReturnTrue_WhenCharacterExistsAndModelIsValid()
    {
		// Arrange
		var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");

        Player player = new()
        {
            Id = 1,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        var id = 1;

        Character character = new()
        {
            Id = id,
            Name = "Ren",
            Race = CharacterRace.Human,
            Health = 100,
            PlayerId = 1,
            Player = player
        };

        _fixture.CharactersRepository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(character);

        // Act
        var sut = _fixture.CreeateSut();

		var result = await sut.PatchAsync(
			id,
			new JsonPatchDocument<CharacterUpdateDto>(),
			_ => true,
			new CancellationToken());

        // Assert
        Assert.IsTrue(result);
	}

	[Test]
	public async Task PatchAsync_Should_ReturnFalse_WhenCharacterExistsAndModelIsNotValid()
	{
    	// Arrange
    	var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");

        Player player = new()
        {
            Id = 1,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        var id = 1;

        Character character = new()
        {
            Id = id,
            Name = "Ren",
            Race = CharacterRace.Human,
            Health = 100,
            PlayerId = 1,
            Player = player
        };

        _fixture.CharactersRepository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(character);

        // Act
        var sut = _fixture.CreeateSut();

        var result = await sut.PatchAsync(
    		id,
    		new JsonPatchDocument<CharacterUpdateDto>(),
    		_ => false,
    		new CancellationToken());

        // Assert
        Assert.IsFalse(result);
	}

	[Test]
	public void PatchAsync_Should_ThrowNullReferenceException_WhenCharacterDoesNotExist()
	{
    	// Arrange
    	var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");

        Player player = new()
        {
            Id = 1,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        var id = 1;

        Character character = new()
        {
            Id = id,
            Name = "Ren",
            Race = CharacterRace.Human,
            Health = 100,
            PlayerId = 1,
            Player = player
        };

        _fixture.CharactersRepository
            .Setup(r => r.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(character);

        // Act
        var sut = _fixture.CreeateSut();

        Task<bool> result() => sut.PatchAsync(
            id,
            new JsonPatchDocument<CharacterUpdateDto>(),
            _ => true,
            new CancellationToken());

        // Assert
        Assert.ThrowsAsync<NullReferenceException>(result);
	}

	[Test]
	[TestCaseSource(nameof(ManageItemAsyncTestData))]
	public void ManageItemAsync_Should_ReturnTask_WhenCharacterAndItemExist(ManageItemDto itemDto)
    {
        // Arrange
        var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");

        Player player = new()
        {
            Id = 1,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        Character character = new()
        {
            Id = 1,
            Name = "Ren",
            Race = CharacterRace.Human,
            Health = 100,
            PlayerId = 1,
            Player = player
        };

        Weapon weapon = new()
        {
            Id = 1,
            Name = "Mighty sword",
            Type = WeaponType.Sword,
            Damage = 65
        };

        Spell spell = new()
        {
            Id = 1,
            Name = "Fireball",
            Type = SpellType.Fire,
            Damage = 17
        };

        Mount mount = new()
        {
            Id = 1,
            Name = "Small donkey",
            Type = MountType.Donkey,
            Speed = 32
        };

		_fixture.CharactersRepository
			.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
			.ReturnsAsync(character);

		_fixture.WeaponsRepository
			.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
			.ReturnsAsync(weapon);

		_fixture.SpellsRepository
			.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
			.ReturnsAsync(spell);

        _fixture.MountsRepository
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mount);

        // Act
        var sut = _fixture.CreeateSut();
        Task result() => sut.ManageItemAsync(itemDto, new CancellationToken());

        // Assert
        Assert.DoesNotThrowAsync(result);
	}

	[Test]
	[TestCaseSource(nameof(ManageItemAsyncTestData))]
	public void ManageItemAsync_Should_ReturnTask_WhenCharacterExistAndItemDoesNotExist(ManageItemDto itemDto)
	{
	    // Arrange
        var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");

        Player player = new()
        {
            Id = 1,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        Character character = new()
        {
            Id = 1,
            Name = "Ren",
            Race = CharacterRace.Human,
            Health = 100,
            PlayerId = 1,
            Player = player
        };

        Weapon weapon = new()
        {
            Id = 1,
            Name = "Mighty sword",
            Type = WeaponType.Sword,
            Damage = 65
        };

        Spell spell = new()
        {
            Id = 1,
            Name = "Fireball",
            Type = SpellType.Fire,
            Damage = 17
        };

        Mount mount = new()
        {
            Id = 1,
            Name = "Small donkey",
            Type = MountType.Donkey,
            Speed = 32
        };

        _fixture.CharactersRepository
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(character);

        _fixture.WeaponsRepository
            .Setup(r => r.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(weapon);

        _fixture.SpellsRepository
            .Setup(r => r.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(spell);

        _fixture.MountsRepository
            .Setup(r => r.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mount);

        // Act
        var sut = _fixture.CreeateSut();
        Task result() => sut.ManageItemAsync(itemDto, new CancellationToken());

        // Assert
        Assert.ThrowsAsync<NullReferenceException>(result);
	}

	[Test]
	[TestCaseSource(nameof(ManageItemAsyncTestData))]
	public void ManageItemAsync_Should_ThrowNullReferenceException_WhenCharacterDoesNotExist(ManageItemDto itemDto)
	{
	    // Arrange
        var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");

        Player player = new()
        {
            Id = 1,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        Character character = new()
        {
            Id = 1,
            Name = "Ren",
            Race = CharacterRace.Human,
            Health = 100,
            PlayerId = 1,
            Player = player
        };

        _fixture.CharactersRepository
            .Setup(r => r.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(character);

        // Act
        var sut = _fixture.CreeateSut();
        Task result() => sut.ManageItemAsync(itemDto, new CancellationToken());

        // Assert
        Assert.ThrowsAsync<NullReferenceException>(result);
	}

	[Test]
	[TestCaseSource(nameof(ManageItemAsyncTestData))]
	public void ManageItemAsync_Should_ThrowNotEnoughRightsException_WhenPlayerDoesNotHaveSufficientRights(
		ManageItemDto itemDto)
	{
	    // Arrange
        var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");

        Player player = new()
        {
            Id = 1,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        Character character = new()
        {
            Id = 1,
            Name = "Ren",
            Race = CharacterRace.Human,
            Health = 100,
            PlayerId = 1,
            Player = player
        };

		_fixture.CharactersRepository
			.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
			.ReturnsAsync(character);

        _fixture.ConfigureAccessHelper(false);

        // Act
        var sut = _fixture.CreeateSut();
        Task result() => sut.ManageItemAsync(itemDto, new CancellationToken());

        // Assert
        Assert.ThrowsAsync<NotEnoughRightsException>(result);
	}

	[Test]
	[TestCaseSource(nameof(HitAsyncSuccessfulTestData))]
	public void HitAsync_Should_ReturnTask_WhenCharacterAndItemExist(
		HitDto hitDto,
		int damage,
		int startingHealth,
		int resultingHealth)
    {
		// Arrange
		var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");

        Player player = new()
        {
            Id = 1,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        Character character = new()
        {
            Id = 1,
            Name = "Ren",
            Race = CharacterRace.Human,
            Health = startingHealth,
            PlayerId = 1,
            Player = player
        };

        Weapon weapon = new()
        {
            Id = 1,
            Name = "Mighty sword",
            Type = WeaponType.Sword,
            Damage = damage
        };

        Spell spell = new()
        {
            Id = 1,
            Name = "Fireball",
            Type = SpellType.Fire,
            Damage = damage
        };

		_fixture.CharactersRepository
			.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
			.ReturnsAsync(character);

		_fixture.WeaponsRepository
			.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
			.ReturnsAsync(weapon);

		_fixture.SpellsRepository
			.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
			.ReturnsAsync(spell);

        // Act
        var sut = _fixture.CreeateSut();
        var result = sut.HitAsync(hitDto, new CancellationToken());

        // Assert
        Assert.AreEqual(resultingHealth, character.Health);
	}

	[Test]
	[TestCaseSource(nameof(HitAsyncFailedTestData))]
	public void HitAsync_Should_ThrowNullReferenceException_WhenCharacterDoesNotExist(HitDto hitDto)
    {
        // Arrange
        var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");

        Player player = new()
        {
            Id = 1,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        Character character = new()
        {
            Id = 1,
            Name = "Ren",
            Race = CharacterRace.Human,
            Health = 100,
            PlayerId = 1,
            Player = player
        };

        _fixture.CharactersRepository
            .Setup(r => r.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(character);

        // Act
        var sut = _fixture.CreeateSut();
        Task result() => sut.HitAsync(hitDto, new CancellationToken());

        // Assert
        Assert.ThrowsAsync<NullReferenceException>(result);
	}

	[Test]
	[TestCaseSource(nameof(HitAsyncFailedTestData))]
	public void HitAsync_Should_ThrowNullReferenceException_WhenCharacterExistsAndItemDoesNotExist(HitDto hitDto)
    {
		// Arrange
		var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");

        Player player = new()
        {
            Id = 1,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        Character character = new()
        {
            Id = 1,
            Name = "Ren",
            Race = CharacterRace.Human,
            Health = 100,
            PlayerId = 1,
            Player = player
        };

        Weapon weapon = new()
        {
            Id = 1,
            Name = "Mighty sword",
            Type = WeaponType.Sword,
            Damage = 65
        };

        Spell spell = new()
        {
            Id = 1,
            Name = "Fireball",
            Type = SpellType.Fire,
            Damage = 17
        };

		_fixture.CharactersRepository
			.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
			.ReturnsAsync(character);

		_fixture.WeaponsRepository
			.Setup(r => r.GetByIdAsync(2, It.IsAny<CancellationToken>()))
			.ReturnsAsync(weapon);

        _fixture.SpellsRepository
            .Setup(r => r.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(spell);

        // Act
        var sut = _fixture.CreeateSut();
        Task result() => sut.HitAsync(hitDto, new CancellationToken());

        // Assert
        Assert.ThrowsAsync<NullReferenceException>(result);
	}

	[Test]
	[TestCaseSource(nameof(HitAsyncFailedTestData))]
	public void HitAsync_Should_ThrowNotEnoughRightsException_WhenPlayerDoesNotHaveSufficientRights(HitDto hitDto)
	{
	// Arrange
	var (hash, salt) = PasswordHelper.GeneratePasswordHashAndSalt("1234Qwer");

        Player player = new()
        {
            Id = 1,
            Name = "Ren",
            Role = PlayerRole.Player,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        Character character = new()
        {
            Id = 1,
            Name = "Ren",
            Race = CharacterRace.Human,
            Health = 100,
            PlayerId = 1,
            Player = player
        };

        Weapon weapon = new()
        {
            Id = 1,
            Name = "Mighty sword",
            Type = WeaponType.Sword,
            Damage = 65
        };

        Spell spell = new()
        {
            Id = 1,
            Name = "Fireball",
            Type = SpellType.Fire,
            Damage = 17
        };

	_fixture.CharactersRepository
		.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
		.ReturnsAsync(character);

	_fixture.WeaponsRepository
		.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
		.ReturnsAsync(weapon);

        _fixture.SpellsRepository
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(spell);

        _fixture.ConfigureAccessHelper(false);

        // Act
        var sut = _fixture.CreeateSut();
        Task result() => sut.HitAsync(hitDto, new CancellationToken());

        // Assert
        Assert.ThrowsAsync<NotEnoughRightsException>(result);
	}

	private static IEnumerable<ManageItemDto> ManageItemAsyncTestData()
	{
		yield return new(1, 1, ItemType.Weapon, ManageItemOperation.Add);
		yield return new(1, 1, ItemType.Weapon, ManageItemOperation.Remove);
		yield return new(1, 1, ItemType.Spell, ManageItemOperation.Add);
		yield return new(1, 1, ItemType.Spell, ManageItemOperation.Remove);
		yield return new(1, 1, ItemType.Mount, ManageItemOperation.Add);
		yield return new(1, 1, ItemType.Mount, ManageItemOperation.Remove);
	}

	private static IEnumerable<object[]> HitAsyncSuccessfulTestData()
	{
		yield return new object[]
		{
			new HitDto(1, 1, 1, HitType.Weapon),
			38,
			100,
			62
		};

		yield return new object[]
		{
			new HitDto(1, 1, 1, HitType.Weapon),
			94,
			75,
			0
		};

		yield return new object[]
		{
			new HitDto(1, 1, 1, HitType.Spell),
			47,
			100,
			53
		};

		yield return new object[]
		{
			new HitDto(1, 1, 1, HitType.Spell),
			28,
			16,
			0
		};
	}

	private static IEnumerable<HitDto> HitAsyncFailedTestData()
	{
		yield return new(1, 1, 1, HitType.Weapon);
		yield return new(1, 1, 1, HitType.Weapon);
		yield return new(1, 1, 1, HitType.Spell);
		yield return new(1, 1, 1, HitType.Spell);
	}
}
