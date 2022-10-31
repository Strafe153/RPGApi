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
public class CharacterServiceTests
{
    private CharacterServiceFixture _fixture = default!;

    [OneTimeSetUp]
    public void SetUp()
    {
        _fixture = new CharacterServiceFixture();
    }

    [TearDown]
    public void TearDown()
    {
        _fixture.Character.Health = 100;
    }

    [Test]
    public async Task GetAllAsync_Should_ReturnPaginatedListOfCharacterFromRepository_WhenPageParametersAreValid()
    {
        // Arrange
        _fixture.CharacterRepository
            .GetAllAsync(Arg.Any<int>(), Arg.Any<int>())
            .Returns(_fixture.PaginatedList);

        // Act
        var result = await _fixture.CharacterService.GetAllAsync(_fixture.Id, _fixture.Id);

        // Assert
        result.Should().NotBeNull().And.NotBeEmpty().And.BeOfType<PaginatedList<Character>>();
    }

    [Test]
    public async Task GetAllAsync_Should_ReturnPaginatedListOfCharacterFromCache_WhenPageParametersAreValid()
    {
        // Arrange
        _fixture.CacheService
            .GetAsync<List<Character>>(Arg.Any<string>())
            .Returns(_fixture.Characters);

        // Act
        var result = await _fixture.CharacterService.GetAllAsync(_fixture.Id, _fixture.Id);

        // Assert
        result.Should().NotBeNull().And.NotBeEmpty().And.BeOfType<PaginatedList<Character>>();
    }

    [Test]
    public async Task GetByIdAsync_Should_ReturnCharacterFromRepository_WhenCharacterExists()
    {
        // Arrange
        _fixture.CharacterRepository
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Character);

        // Act
        var result = await _fixture.CharacterService.GetByIdAsync(_fixture.Id);

        // Assert
        result.Should().NotBeNull().And.BeOfType<Character>();
    }

    [Test]
    public async Task GetByIdAsync_Should_ReturnCharacterFromCache_WhenCharacterExists()
    {
        // Arrange
        _fixture.CacheService
            .GetAsync<Character>(Arg.Any<string>())
            .Returns(_fixture.Character);

        // Act
        var result = await _fixture.CharacterService.GetByIdAsync(_fixture.Id);

        // Assert
        result.Should().NotBeNull().And.BeOfType<Character>();
    }

    [Test]
    public async Task GetByIdAsync_Should_ThrowNullReferenceException_WhenCharacterDoesNotExist()
    {
        // Arrange
        _fixture.CacheService
            .GetAsync<Character>(Arg.Any<string>())
            .ReturnsNull();

        _fixture.CharacterRepository
            .GetByIdAsync(Arg.Any<int>())
            .ReturnsNull();

        // Act
        var result = async () => await _fixture.CharacterService.GetByIdAsync(_fixture.Id);

        // Assert
        await result.Should().ThrowAsync<NullReferenceException>();
    }

    [Test]
    public void AddAsync_Should_ReturnTask_WhenCharacterIsValid()
    {
        // Act
        var result = _fixture.CharacterService.AddAsync(_fixture.Character);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void UpdateAsync_Should_ReturnTAsk_WhenCharacterIsValid()
    {
        // Act
        var result = _fixture.CharacterService.UpdateAsync(_fixture.Character);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void DeleteAsync_Should_ReturnTask_WhenCharacterIsValid()
    {
        // Act
        var result = _fixture.CharacterService.DeleteAsync(_fixture.Id);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void GetWeapon_Should_ReturnWeapon_WhenWeaponExists()
    {
        // Act
        var result = _fixture.CharacterService.GetWeapon(_fixture.Character, _fixture.Id);

        // Assert
        result.Should().NotBeNull().And.BeOfType<Weapon>();
    }

    [Test]
    public void GetWeapon_Should_ThrowItemNotFoundException_WhenWeaponDoesNotExist()
    {
        // Act
        var result = () => _fixture.CharacterService.GetWeapon(_fixture.Character, 3);

        // Assert
        result.Should().Throw<ItemNotFoundException>();
    }

    [Test]
    public void GetSpell_Should_ReturnSpell_WhenSpellExists()
    {
        // Act
        var result = _fixture.CharacterService.GetSpell(_fixture.Character, _fixture.Id);

        // Assert
        result.Should().NotBeNull().And.BeOfType<Spell>();
    }

    [Test]
    public void GetSpell_Should_ThrowItemNotFoundException_WhenSpellDoesNotExist()
    {
        // Act
        var result = () => _fixture.CharacterService.GetSpell(_fixture.Character, 3);

        // Assert
        result.Should().Throw<ItemNotFoundException>();
    }

    [TestCase(20, 80)]
    [TestCase(100, 0)]
    [TestCase(-25, 100)]
    public void CalculateHealth_Should_ReturnRemainingHealth_WhenDamaged(int damage, int resultingHealth)
    {
        // Act
        _fixture.CharacterService.CalculateHealth(_fixture.Character, damage);

        // Assert
        resultingHealth.Should().Be(_fixture.Character.Health);
    }
}
