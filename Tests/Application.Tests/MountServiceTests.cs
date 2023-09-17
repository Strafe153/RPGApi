using Application.Tests.Fixtures;
using Core.Entities;
using Core.Exceptions;
using Core.Shared;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;

namespace Application.Tests;

[TestFixture]
public class MountServiceTests
{
    private MountServiceFixture _fixture = default!;

    [SetUp]
    public void SetUp()
    {
        _fixture = new MountServiceFixture();
    }

    [Test]
    public async Task GetAllAsync_Should_ReturnPaginatedListOfMounts_WhenParametersAreValid()
    {
        // Arrange
        _fixture.MountRepository
            .GetAllAsync(Arg.Any<int>(), Arg.Any<int>())
            .Returns(_fixture.PaginatedList);

        // Act
        var result = await _fixture.MountService.GetAllAsync(_fixture.PageNumber, _fixture.PageSize);

        // Assert
        result.Should().NotBeNull().And.NotBeEmpty().And.BeOfType<PaginatedList<Mount>>();
    }

    [Test]
    public async Task GetByIdAsync_Should_ReturnMount_WhenMountsExists()
    {
        // Arrange
        _fixture.MountRepository
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Mount);

        // Act
        var result = await _fixture.MountService.GetByIdAsync(_fixture.Id);

        // Assert
        result.Should().NotBeNull().And.BeOfType<Mount>();
    }

    [Test]
    public async Task GetByIdAsync_Should_ThrowNullReferenceException_WhenMountDoesNotExist()
    {
        // Arrange
        _fixture.MountRepository
            .GetByIdAsync(Arg.Any<int>())
            .ReturnsNull();

        // Act
        var result = async () => await _fixture.MountService.GetByIdAsync(_fixture.Id);

        // Assert
        await result.Should().ThrowAsync<NullReferenceException>();
    }

    [Test]
    public void AddAsync_Should_ReturnTask_WhenMountIsValid()
    {
        // Act
        var result = _fixture.MountService.AddAsync(_fixture.Mount);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void UpdateAsync_Should_ReturnTask_WhenMountIsValid()
    {
        // Act
        var result = _fixture.MountService.UpdateAsync(_fixture.Mount);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void DeleteAsync_Should_ReturnTask_WhenMountIsValid()
    {
        // Act
        var result = _fixture.MountService.DeleteAsync(_fixture.Id);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void AddToCharacter_Should_ReturnVoid_WhenDataIsValid()
    {
        // Act
        var result = () => _fixture.MountService.AddToCharacterAsync(_fixture.Character, _fixture.Mount);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void RemoveFromCharacter_Should_ReturnVoid_WhenCharacterMountExist()
    {
        // Act
        var result = async () => await _fixture.MountService.RemoveFromCharacterAsync(_fixture.Character, _fixture.Mount);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void RemoveFromCharacter_Should_ThrowItemNotFoundException_WhenCharacterMountDoesNotExist()
    {
        // Act
        var result = async () => await _fixture.MountService.RemoveFromCharacterAsync(_fixture.Character, _fixture.Mount);

        // Assert
        result.Should().ThrowAsync<ItemNotFoundException>();
    }
}
