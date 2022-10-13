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
    public async Task GetAllAsync_ValidParametersDataFromRepository_ReturnsPaginatedListOfCharacter()
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
    public async Task GetAllAsync_ValidParametersDataFromCache_ReturnsPaginatedListOfCharacter()
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
    public async Task GetByIdAsync_ExistingCharacterInRepository_ReturnsCharacter()
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
    public async Task GetByIdAsync_ExistingCharacterInCache_ReturnsCharacter()
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
    public async Task GetByIdAsync_NonexistingCharacter_ThrowsNullReferenceException()
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
    public void AddAsync_ValidCharacter_ReturnsTask()
    {
        // Act
        var result = _fixture.CharacterService.AddAsync(_fixture.Character);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void UpdateAsync_ValidCharacter_ReturnsTask()
    {
        // Act
        var result = _fixture.CharacterService.UpdateAsync(_fixture.Character);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void DeleteAsync_ValidCharacter_ReturnsTask()
    {
        // Act
        var result = _fixture.CharacterService.DeleteAsync(_fixture.Id);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void GetWeapon_ExistingWeapon_ReturnsWeapon()
    {
        // Act
        var result = _fixture.CharacterService.GetWeapon(_fixture.Character, _fixture.Id);

        // Assert
        result.Should().NotBeNull().And.BeOfType<Weapon>();
    }

    [Test]
    public void GetWeapon_NonexistingWeapon_ThrowsItemNotFoundException()
    {
        // Act
        var result = () => _fixture.CharacterService.GetWeapon(_fixture.Character, 3);

        // Assert
        result.Should().Throw<ItemNotFoundException>();
    }

    [Test]
    public void GetSpell_ExistingSpell_ReturnsSpell()
    {
        // Act
        var result = _fixture.CharacterService.GetSpell(_fixture.Character, _fixture.Id);

        // Assert
        result.Should().NotBeNull().And.BeOfType<Spell>();
    }

    [Test]
    public void GetSpell_NonexistingSpell_ThrowsItemNotFoundException()
    {
        // Act
        var result = () => _fixture.CharacterService.GetSpell(_fixture.Character, 3);

        // Assert
        result.Should().Throw<ItemNotFoundException>();
    }

    [TestCase(20, 80)]
    [TestCase(100, 0)]
    [TestCase(-25, 100)]
    public void CalculateHealth_VariousDamage_ReturnsRemainingHealth(int damage, int resultingHealth)
    {
        // Act
        _fixture.CharacterService.CalculateHealth(_fixture.Character, damage);

        // Assert
        resultingHealth.Should().Be(_fixture.Character.Health);
    }
}
