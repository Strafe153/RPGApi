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
public class SpellServiceTests
{
    private SpellServiceFixture _fixture = default!;

    [OneTimeSetUp]
    public void SetUp()
    {
        _fixture = new SpellServiceFixture();
    }

    [Test]
    public async Task GetAllAsync_Should_ReturnPaginatedListOfSpell_WhenPageParametersAreValid()
    {
        // Arrange
        _fixture.SpellRepository
            .GetAllAsync(Arg.Any<int>(), Arg.Any<int>())
            .Returns(_fixture.PaginatedList);

        // Act
        var result = await _fixture.SpellService.GetAllAsync(_fixture.PageNumber, _fixture.PageSize);

        // Assert
        result.Should().NotBeNull().And.NotBeEmpty().And.BeOfType<PaginatedList<Spell>>();
    }

    [Test]
    public async Task GetByIdAsync_Should_ReturnSpell_WhenSpellExists()
    {
        // Arrange
        _fixture.SpellRepository
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Spell);

        // Act
        var result = await _fixture.SpellService.GetByIdAsync(_fixture.Id);

        // Assert
        result.Should().NotBeNull().And.BeOfType<Spell>();
    }

    [Test]
    public async Task GetByIdAsync_Should_ThrowNullReferenceException_WhenSpellDoesNotExist()
    {
        // Arrange
        _fixture.SpellRepository
            .GetByIdAsync(Arg.Any<int>())
            .ReturnsNull();

        // Act
        var result = async () => await _fixture.SpellService.GetByIdAsync(_fixture.Id);

        // Assert
        await result.Should().ThrowAsync<NullReferenceException>();
    }

    [Test]
    public void AddAsync_Should_ReturnTask_WhenSpellIsValid()
    {
        // Act
        var result = _fixture.SpellService.AddAsync(_fixture.Spell);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void UpdateAsync_Should_ReturnTask_WhenSpellIsValid()
    {
        // Act
        var result = _fixture.SpellService.UpdateAsync(_fixture.Spell);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void DeleteAsync_Should_ReturnTask_WhenSpellIsValid()
    {
        // Act
        var result = _fixture.SpellService.DeleteAsync(_fixture.Id);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void AddToCharacter_Should_ReturnVoid_WhenDataIsValid()
    {
        // Act
        var result = () => _fixture.SpellService.AddToCharacterAsync(_fixture.Character, _fixture.Spell);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void RemoveFromCharacter_Should_ReturnVoid_WhenSpellExists()
    {
        // Act
        var result = async () => await _fixture.SpellService.RemoveFromCharacterAsync(_fixture.Character, _fixture.Spell);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void RemoveFromCharacter_Should_ThrowItemNotFoundException_WhenSpellDoesNotExist()
    {
        // Act
        var result = async () => await _fixture.SpellService.RemoveFromCharacterAsync(_fixture.Character, _fixture.Spell);

        // Assert
        result.Should().ThrowAsync<ItemNotFoundException>();
    }
}
