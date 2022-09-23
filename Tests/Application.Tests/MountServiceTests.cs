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
public class MountServiceTests
{
    private MountServiceFixture _fixture = default!;

    [OneTimeSetUp]
    public void SetUp()
    {
        _fixture = new MountServiceFixture();
    }

    [Test]
    public async Task GetAllAsync_ValidParameters_ReturnsPaginatedListOfMount()
    {
        // Arrange
        _fixture.MountRepository
            .GetAllAsync(Arg.Any<int>(), Arg.Any<int>())
            .Returns(_fixture.PaginatedList);

        // Act
        var result = await _fixture.MountService.GetAllAsync(_fixture.Id, _fixture.Id);

        // Assert
        result.Should().NotBeNull().And.NotBeEmpty().And.BeOfType<PaginatedList<Mount>>();
    }

    [Test]
    public async Task GetByIdAsync_ExistingMount_ReturnsMount()
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
    public async Task GetByIdAsync_NonexistingMount_ThrowsNullReferenceException()
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
    public void AddAsync_ValidMount_ReturnsTask()
    {
        // Act
        var result = _fixture.MountService.AddAsync(_fixture.Mount);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void UpdateAsync_ValidMount_ReturnsTask()
    {
        // Act
        var result = _fixture.MountService.UpdateAsync(_fixture.Mount);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void DeleteAsync_ValidMount_ReturnsTask()
    {
        // Act
        var result = _fixture.MountService.DeleteAsync(_fixture.Mount);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void AddToCharacter_ValidData_ReturnsVoid()
    {
        // Act
        var result = () => _fixture.MountService.AddToCharacter(_fixture.Character, _fixture.Mount);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void RemoveFromCharacter_ExistingCharacterMount_ReturnsVoid()
    {
        // Act
        var result = () => _fixture.MountService.RemoveFromCharacter(_fixture.Character, _fixture.Mount);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void RemoveFromCharacter_NonexistingCharacterMount_ThrowsItemNotFoundException()
    {
        // Act
        var result = () => _fixture.MountService.RemoveFromCharacter(_fixture.Character, _fixture.Mount);

        // Assert
        result.Should().Throw<ItemNotFoundException>();
    }
}
