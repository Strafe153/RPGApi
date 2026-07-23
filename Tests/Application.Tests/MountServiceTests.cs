using Application.Dtos.MountDtos;
using Application.Tests.Fixtures;
using Domain.Entities;
using Domain.Enums;
using Domain.Shared;
using Microsoft.AspNetCore.JsonPatch;
using Moq;
using NUnit.Framework;

namespace Application.Tests;

[TestFixture]
public class MountServiceTests
{
	private MountServiceFixture _fixture = default!;

	[SetUp]
	public void SetUp() => _fixture = new();

	[Test]
	public async Task GetAllAsync_Should_ReturnPageDtoOfMountReadDto_WhenDataIsValid()
    {
        // Arrange
        List<Mount> mounts = [
            new()
            {
                Id = 1,
                Name = "Max Elephant",
                Type = MountType.Elephant,
                Speed = 5
            },
            new()
            {
                Id = 2,
                Name = "Bigby",
                Type = MountType.Wolf,
                Speed = 69
            },
        ];

        PagedList<Mount> pagedList = new(mounts, mounts.Count, 1, 2);

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
		Assert.AreEqual(5, result.Entities.First().Speed);
		Assert.AreEqual("Bigby", result.Entities.Last().Name);
	}

	[Test]
	public async Task GetByIdAsync_Should_ReturnMountReadDto_WhenMountsExists()
    {
        // Arrange
        var id = 1;

        Mount mount = new()
        {
            Id = id,
            Name = "Bigby",
            Type = MountType.Wolf,
            Speed = 69
        };

        _fixture.Repository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mount);

        // Act
        var sut = _fixture.CreateSut();
        var result = await sut.GetByIdAsync(id, new CancellationToken());

        // Assert
        Assert.AreEqual(MountType.Wolf, result.Type);
    }

	[Test]
	public void GetByIdAsync_Should_ThrowNullReferenceException_WhenMountDoesNotExist()
	{
	    // Arrange
        var id = 1;

        Mount mount = new()
        {
            Id = id,
            Name = "Bigby",
            Type = MountType.Wolf,
            Speed = 69
        };

        _fixture.Repository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mount);

        // Act
        var sut = _fixture.CreateSut();
        Task<MountReadDto> result() => sut.GetByIdAsync(2, new CancellationToken());

        // Assert
        Assert.ThrowsAsync<NullReferenceException>(result);
	}

	[Test]
	public async Task AddAsync_Should_ReturnMountReadDto_WhenDtoIsValid()
    {
        // Arrange
        MountCreateDto createDto = new(
            "Bigby",
            MountType.Wolf,
            69);

        // Act
        var sut = _fixture.CreateSut();
        var result = await sut.AddAsync(createDto);

        // Assert
        Assert.AreEqual(createDto.Name, result.Name);
        Assert.AreEqual(createDto.Speed, result.Speed);
	}

	[Test]
	public void UpdateAsync_Should_ReturnTask_WhenMountExists()
    {
        // Arrange
        var id = 1;

        Mount mount = new()
        {
            Id = id,
            Name = "Bigby",
            Type = MountType.Wolf,
            Speed = 69
        };

        MountUpdateDto updateDto = new(
            "Bigby",
            MountType.Wolf,
            69);

        _fixture.Repository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mount);

        // Act
        var sut = _fixture.CreateSut();
        Task result() => sut.UpdateAsync(1, updateDto, new CancellationToken());

        // Assert
        Assert.DoesNotThrowAsync(result);
    }

	[Test]
	public void UpdateAsync_Should_ThrowNullReferenceExcpetion_WhenMountDoesNotExist()
	{
	    // Arrange
        var id = 1;

        Mount mount = new()
        {
            Id = id,
            Name = "Bigby",
            Type = MountType.Wolf,
            Speed = 69
        };

        MountUpdateDto updateDto = new(
            "Bigby",
            MountType.Wolf,
            69);

        _fixture.Repository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mount);

        // Act
        var sut = _fixture.CreateSut();
        Task result() => sut.UpdateAsync(2, updateDto, new CancellationToken());

        // Assert
        Assert.ThrowsAsync<NullReferenceException>(result);
	}

	[Test]
	public void DeleteAsync_Should_ReturnTask_WhenMountExists()
    {
		// Arrange
		var id = 1;

        Mount mount = new()
        {
            Id = id,
            Name = "Bigby",
            Type = MountType.Wolf,
            Speed = 69
        };

        _fixture.Repository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mount);

        // Act
        var sut = _fixture.CreateSut();
        Task result() => sut.DeleteAsync(1, new CancellationToken());

        // Assert
        Assert.DoesNotThrowAsync(result);
    }

	[Test]
	public void DeleteAsync_Should_ThrowNullReferenceException_WhenMountDoesNotExist()
	{
    	// Arrange
    	var id = 1;

        Mount mount = new()
        {
            Id = id,
            Name = "Bigby",
            Type = MountType.Wolf,
            Speed = 69
        };

        _fixture.Repository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mount);

        // Act
        var sut = _fixture.CreateSut();
        Task result() => sut.DeleteAsync(2, new CancellationToken());

        // Assert
        Assert.ThrowsAsync<NullReferenceException>(result);
	}

	[Test]
	public async Task PatchAsync_Should_ReturnTrue_WhenMountExistsAndModelIsValid()
    {
        // Arrange
        var id = 1;

        Mount mount = new()
        {
            Id = id,
            Name = "Bigby",
            Type = MountType.Wolf,
            Speed = 69
        };

        _fixture.Repository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mount);

        // Act
        var sut = _fixture.CreateSut();

		var result = await sut.PatchAsync(
			1,
			new JsonPatchDocument<MountUpdateDto>(),
			_ => true,
			new CancellationToken());

        // Assert
        Assert.IsTrue(result);
	}

	[Test]
	public async Task PatchAsync_Should_ReturnFalse_WhenMountExistsAndModelIsNotValid()
	{
	    // Arrange
        var id = 1;

        Mount mount = new()
        {
            Id = id,
            Name = "Bigby",
            Type = MountType.Wolf,
            Speed = 69
        };

        _fixture.Repository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mount);

        // Act
        var sut = _fixture.CreateSut();

        var result = await sut.PatchAsync(
            1,
            new JsonPatchDocument<MountUpdateDto>(),
            _ => false,
            new CancellationToken());

        // Assert
        Assert.IsFalse(result);
	}

	[Test]
	public void PatchAsync_Should_ThrowNullReferenceException_WhenMountDoesNotExist()
	{
	    // Arrange
        var id = 1;

        Mount mount = new()
        {
            Id = id,
            Name = "Bigby",
            Type = MountType.Wolf,
            Speed = 69
        };

        _fixture.Repository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mount);

        // Act
        var sut = _fixture.CreateSut();

        Task<bool> result() => sut.PatchAsync(
            2,
            new JsonPatchDocument<MountUpdateDto>(),
            _ => true,
            new CancellationToken());

        // Assert
        Assert.ThrowsAsync<NullReferenceException>(result);
	}
}
