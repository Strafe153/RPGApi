using Application.Dtos.WeaponDtos;
using Application.Tests.Fixtures;
using Domain.Entities;
using Domain.Enums;
using Domain.Shared;
using Microsoft.AspNetCore.JsonPatch;
using Moq;
using NUnit.Framework;

namespace Application.Tests;

[TestFixture]
public class WeaponServiceTests
{
	private WeaponServiceFixture _fixture = default!;

	[SetUp]
	public void SetUp() => _fixture = new();

	[Test]
	public async Task GetAllAsync_Should_ReturnPageDtoOfWeaponReadDto_WhenDataIsValid()
    {
		// Arrange
		List<Weapon> weapons = [
    		new()
            {
                Id = 1,
                Name = "Iron Bow",
                Type = WeaponType.Bow,
                Damage = 21
            },
            new()
            {
                Id = 2,
                Name = "Rebel Knife",
                Type = WeaponType.Dagger,
                Damage = 87
            },
        ];

        PagedList<Weapon> pagedList = new(weapons, weapons.Count, 1, 2);

        PageParameters pageParameters = new()
        {
            PageNumber = 1,
            PageSize = 2
        };

        _fixture.Repository
            .Setup(r => r.GetAllAsync(It.IsAny<PageParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedList);

        // Act
        var sut = _fixture.CreateSut();
		var result = await sut.GetAllAsync(pageParameters, new CancellationToken());

		// Assert
		Assert.AreEqual(21, result.Entities.First().Damage);
		Assert.AreEqual("Rebel Knife", result.Entities.Last().Name);
	}

	[Test]
	public async Task GetByIdAsync_Should_ReturnWeaponReadDto_WhenWeaponExists()
    {
        // Arrange
        var id = 1;

        Weapon weapon = new()
        {
            Id = id,
            Name = "Rebel Knife",
            Type = WeaponType.Dagger,
            Damage = 87
        };

        _fixture.Repository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(weapon);

        // Act
        var sut = _fixture.CreateSut();
        var result = await sut.GetByIdAsync(1, new CancellationToken());

        // Assert
        Assert.AreEqual(WeaponType.Dagger, result.Type);
	}

	[Test]
	public void GetByIdAsync_Should_ThrowNullReferenceException_WhenWeaponDoesNotExist()
	{
	    // Arrange
        var id = 1;

        Weapon weapon = new()
        {
            Id = id,
            Name = "Rebel Knife",
            Type = WeaponType.Dagger,
            Damage = 87
        };

        _fixture.Repository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(weapon);

        // Act
        var sut = _fixture.CreateSut();
        Task<Dtos.WeaponDtos.WeaponReadDto> result() => sut.GetByIdAsync(2, new CancellationToken());

        // Assert
        Assert.ThrowsAsync<NullReferenceException>(result);
    }

	[Test]
	public async Task AddAsync_Should_ReturnWeaponReadDto_WhenDtoIsValid()
    {
        // Act
        WeaponCreateDto createDto = new("Rebel Knife", WeaponType.Dagger, 87);

        var sut = _fixture.CreateSut();
        var result = await sut.AddAsync(createDto);

        // Assert
        Assert.AreEqual("Rebel Knife", createDto.Name);
	}

	[Test]
	public void UpdateAsync_Should_ReturnTask_WhenWeaponExists()
    {
        // Arrange
        var id = 1;

        Weapon weapon = new()
        {
            Id = id,
            Name = "Rebel Knife",
            Type = WeaponType.Dagger,
            Damage = 87
        };

        WeaponUpdateDto updateDto = new("Rebel Knife", WeaponType.Dagger, 87);

        _fixture.Repository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(weapon);

        // Act
        var sut = _fixture.CreateSut();
        Task result() => sut.UpdateAsync(1, updateDto, new CancellationToken());

        // Assert
        Assert.DoesNotThrowAsync(result);
    }

	[Test]
	public void UpdateAsync_Should_ThrowNullReferenceException_WhenWeaponDoesNotExist()
	{
	    // Arrange
        var id = 1;

        Weapon weapon = new()
        {
            Id = id,
            Name = "Rebel Knife",
            Type = WeaponType.Dagger,
            Damage = 87
        };

        WeaponUpdateDto updateDto = new("Rebel Knife", WeaponType.Dagger, 87);

        _fixture.Repository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(weapon);

        // Act
        var sut = _fixture.CreateSut();
        Task result() => sut.UpdateAsync(2, updateDto, new CancellationToken());

        // Assert
        Assert.ThrowsAsync<NullReferenceException>(result);
	}

	[Test]
	public void DeleteAsync_Should_ReturnTask_WhenWeaponExists()
    {
        // Arrange
        var id = 1;

        Weapon weapon = new()
        {
            Id = id,
            Name = "Rebel Knife",
            Type = WeaponType.Dagger,
            Damage = 87
        };

        _fixture.Repository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(weapon);

        // Act
        var sut = _fixture.CreateSut();
        Task result() => sut.DeleteAsync(1, new CancellationToken());

        // Assert
        Assert.DoesNotThrowAsync(result);
    }

	[Test]
	public void DeleteAsync_Should_ThrowNullReferenceException_WhenWeaponDoesNotExist()
	{
	    // Arrange
        var id = 1;

        Weapon weapon = new()
        {
            Id = id,
            Name = "Rebel Knife",
            Type = WeaponType.Dagger,
            Damage = 87
        };

        _fixture.Repository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(weapon);

        // Act
        var sut = _fixture.CreateSut();
        Task result() => sut.DeleteAsync(2, new CancellationToken());

        // Assert
        Assert.ThrowsAsync<NullReferenceException>(result);
	}

	[Test]
	public async Task PatchAsync_Should_ReturnTrue_WhenWeaponExistsAndModelIsValid()
    {
		// Arrange
		var id = 1;

        Weapon weapon = new()
        {
            Id = id,
            Name = "Rebel Knife",
            Type = WeaponType.Dagger,
            Damage = 87
        };

        _fixture.Repository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(weapon);

        // Act
        var sut = _fixture.CreateSut();

        var result = await sut.PatchAsync(
            1,
            new JsonPatchDocument<WeaponUpdateDto>(),
            _ => true,
            new CancellationToken());

        // Assert
        Assert.IsTrue(result);
	}

	[Test]
	public async Task PatchAsync_Should_ReturnFalse_WhenWeaponExistsAndModelIsNotValid()
	{
    	// Arrange
    	var id = 1;

        Weapon weapon = new()
        {
            Id = id,
            Name = "Rebel Knife",
            Type = WeaponType.Dagger,
            Damage = 87
        };

        _fixture.Repository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(weapon);

        // Act
        var sut = _fixture.CreateSut();

        var result = await sut.PatchAsync(
            1,
            new JsonPatchDocument<WeaponUpdateDto>(),
            _ => false,
            new CancellationToken());

        // Assert
        Assert.IsFalse(result);
	}

	[Test]
	public void PatchAsync_Should_ThrowNullReferenceException_WhenWeaponDoesNotExist()
	{
    	// Arrange
    	var id = 1;

        Weapon weapon = new()
        {
            Id = id,
            Name = "Rebel Knife",
            Type = WeaponType.Dagger,
            Damage = 87
        };

        _fixture.Repository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(weapon);

        // Act
        var sut = _fixture.CreateSut();

        Task<bool> result() => sut.PatchAsync(
            2,
            new JsonPatchDocument<WeaponUpdateDto>(),
            _ => true,
            new CancellationToken());

        // Assert
        Assert.ThrowsAsync<NullReferenceException>(result);
	}
}
