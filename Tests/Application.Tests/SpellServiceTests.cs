using Application.Dtos.SpellDtos;
using Application.Tests.Fixtures;
using Domain.Entities;
using Domain.Enums;
using Domain.Shared;
using Microsoft.AspNetCore.JsonPatch;
using Moq;
using NUnit.Framework;

namespace Application.Tests;

[TestFixture]
public class SpellServiceTests
{
	private SpellServiceFixture _fixture = default!;

	[SetUp]
	public void SetUp() => _fixture = new();

	[Test]
	public async Task GetAllAsync_Should_ReturnPageDtoOfSpellReadDto_WhenDataIsValid()
    {
		// Arrange
		List<Spell> spells = [
    		new()
            {
                Id = 1,
                Name = "Splash",
                Type = SpellType.Water,
                Damage = 8
            },
            new()
            {
                Id = 2,
                Name = "Lightning",
                Type = SpellType.Thunder,
                Damage = 44
            },
        ];

        PagedList<Spell> pagedList = new(spells, spells.Count, 1, 2);

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
		Assert.AreEqual("Splash", result.Entities.First().Name);
		Assert.AreEqual(44, result.Entities.Last().Damage);
	}

	[Test]
	public async Task GetByIdAsync_Should_ReturnSpellReadDto_WhenSpellExists()
    {
        // Arrange
        var id = 1;

        Spell spell = new()
        {
            Id = id,
            Name = "Splash",
            Type = SpellType.Water,
            Damage = 8
        };

        _fixture.Repository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(spell);

        // Act
        var sut = _fixture.CreateSut();
        var result = await sut.GetByIdAsync(1, new CancellationToken());

        // Assert
        Assert.AreEqual(SpellType.Water, result.Type);
        Assert.AreEqual("Splash", result.Name);
	}

	[Test]
	public void GetByIdAsync_Should_ThrowNullReferenceException_WhenSpellDoesNotExist()
	{
	    // Arrange
        var id = 1;

        Spell spell = new()
        {
            Id = id,
            Name = "Splash",
            Type = SpellType.Water,
            Damage = 8
        };

        _fixture.Repository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(spell);

        // Act
        var sut = _fixture.CreateSut();
        Task<SpellReadDto> result() => sut.GetByIdAsync(2, new CancellationToken());

        // Assert
        Assert.ThrowsAsync<NullReferenceException>(result);
	}

	[Test]
	public async Task AddAsync_Should_ReturnSpellReadDto_WhenDtoIsValid()
    {
        // Act
        SpellCreateDto createDto = new("Splash", SpellType.Water, 8);

        var sut = _fixture.CreateSut();
        var result = await sut.AddAsync(createDto);

        // Assert
        Assert.AreEqual(createDto.Name, result.Name);
	}

	[Test]
	public void UpdateAsync_Should_ReturnTask_WhenSpellExists()
    {
        // Arrange
        var id = 1;

        Spell spell = new()
        {
            Id = id,
            Name = "Splash",
            Type = SpellType.Water,
            Damage = 8
        };

        SpellUpdateDto updateDto = new("Splash", SpellType.Water, 8);

        _fixture.Repository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(spell);

        // Act
        var sut = _fixture.CreateSut();
        Task result() => sut.UpdateAsync(1, updateDto, new CancellationToken());

        // Assert
        Assert.DoesNotThrowAsync(result);
	}

	[Test]
	public void UpdateAsync_Should_ThrowNullReferenceException_WhenSpellDoesNotExist()
	{
	    // Arrange
        var id = 1;

        Spell spell = new()
        {
            Id = id,
            Name = "Splash",
            Type = SpellType.Water,
            Damage = 8
        };

        SpellUpdateDto updateDto = new("Splash", SpellType.Water, 8);

        _fixture.Repository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(spell);

        // Act
        var sut = _fixture.CreateSut();
        Task result() => sut.UpdateAsync(2, updateDto, new CancellationToken());

        // Assert
        Assert.ThrowsAsync<NullReferenceException>(result);
	}

	[Test]
	public void DeleteAsync_Should_ReturnTask_WhenSpellExists()
    {
		// Arrange
        var id = 1;

        Spell spell = new()
        {
            Id = id,
            Name = "Splash",
            Type = SpellType.Water,
            Damage = 8
        };

        _fixture.Repository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(spell);

        // Act
        var sut = _fixture.CreateSut();
        Task result() => sut.DeleteAsync(1, new CancellationToken());

        // Assert
        Assert.DoesNotThrowAsync(result);
    }

	[Test]
	public void DeleteAsync_Should_ThrowNullReferenceException_WhenSpellDoesNotExist()
	{
	    // Arrange
        var id = 1;

        Spell spell = new()
        {
            Id = id,
            Name = "Splash",
            Type = SpellType.Water,
            Damage = 8
        };

        _fixture.Repository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(spell);

        // Act
        var sut = _fixture.CreateSut();
        Task result() => sut.DeleteAsync(2, new CancellationToken());

        // Assert
        Assert.ThrowsAsync<NullReferenceException>(result);
	}

	[Test]
	public async Task PatchAsync_Should_ReturnTrue_WhenSpellExistsAndModelIsValid()
    {
		// Arrange
		var id = 1;

        Spell spell = new()
        {
            Id = id,
            Name = "Splash",
            Type = SpellType.Water,
            Damage = 8
        };

        _fixture.Repository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(spell);

        // Act
        var sut = _fixture.CreateSut();

        var result = await sut.PatchAsync(
            1,
            new JsonPatchDocument<SpellUpdateDto>(),
            _ => true,
            new CancellationToken());

        // Assert
        Assert.IsTrue(result);
    }

	[Test]
	public async Task PatchAsync_Should_ReturnFalse_WhenSpellExistsAndModelIsNotValid()
	{
    	// Arrange
    	var id = 1;

        Spell spell = new()
        {
            Id = id,
            Name = "Splash",
            Type = SpellType.Water,
            Damage = 8
        };

        _fixture.Repository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(spell);

        // Act
        var sut = _fixture.CreateSut();

        var result = await sut.PatchAsync(
            1,
            new JsonPatchDocument<SpellUpdateDto>(),
            _ => false,
            new CancellationToken());

        // Assert
        Assert.IsFalse(result);
	}

	[Test]
	public void PatchAsync_Should_ThrowNullReferenceException_WhenSpellDoesNotExist()
	{
    	// Arrange
    	var id = 1;

        Spell spell = new()
        {
            Id = id,
            Name = "Splash",
            Type = SpellType.Water,
            Damage = 8
        };

        _fixture.Repository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(spell);

        // Act
        var sut = _fixture.CreateSut();

        Task<bool> result() => sut.PatchAsync(
            2,
            new JsonPatchDocument<SpellUpdateDto>(),
            _ => true,
            new CancellationToken());

        // Assert
        Assert.ThrowsAsync<NullReferenceException>(result);
	}
}
