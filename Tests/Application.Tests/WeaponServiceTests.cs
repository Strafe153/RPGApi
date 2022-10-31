﻿using Application.Tests.Fixtures;
using Core.Entities;
using Core.Exceptions;
using Core.Shared;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;

namespace Application.Tests;

[TestFixture]
public class WeaponServiceTests
{
    private WeaponServiceFixture _fixture = default!;

    [OneTimeSetUp]
    public void SetUp()
    {
        _fixture = new WeaponServiceFixture();
    }

    [Test]
    public async Task GetAllAsync_Should_ReturnPaginatedListOfWeaponFromRepository_WhenParametersAreValid()
    {
        // Arrange
        _fixture.WeaponRepository
            .GetAllAsync(Arg.Any<int>(), Arg.Any<int>())
            .Returns(_fixture.PaginatedList);

        // Act
        var result = await _fixture.WeaponService.GetAllAsync(_fixture.Id, _fixture.Id);

        // Assert
        result.Should().NotBeNull().And.NotBeEmpty().And.BeOfType<PaginatedList<Weapon>>();
    }

    [Test]
    public async Task GetAllAsync_Should_ReturnPaginatedListOfWeaponFromCache_WhenParametersAreValid()
    {
        // Arrange
        _fixture.CacheService
            .GetAsync<List<Weapon>>(Arg.Any<string>())
            .Returns(_fixture.Weapons);

        // Act
        var result = await _fixture.WeaponService.GetAllAsync(_fixture.Id, _fixture.Id);

        // Assert
        result.Should().NotBeNull().And.NotBeEmpty().And.BeOfType<PaginatedList<Weapon>>();
    }

    [Test]
    public async Task GetByIdAsync_Should_ReturnWeaponFromRepository_WhenWeaponExists()
    {
        // Arrange
        _fixture.WeaponRepository
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Weapon);

        // Act
        var result = await _fixture.WeaponService.GetByIdAsync(_fixture.Id);

        // Assert
        result.Should().NotBeNull().And.BeOfType<Weapon>();
    }

    [Test]
    public async Task GetByIdAsync_Should_ReturnWeaponFromCache_WhenWeaponExists()
    {
        // Arrange
        _fixture.CacheService
            .GetAsync<Weapon>(Arg.Any<string>())
            .Returns(_fixture.Weapon);

        // Act
        var result = await _fixture.WeaponService.GetByIdAsync(_fixture.Id);

        // Assert
        result.Should().NotBeNull().And.BeOfType<Weapon>();
    }

    [Test]
    public async Task GetByIdAsync_Should_ThrowNullReferenceException_WhenWeaponDoesNotExist()
    {
        // Arrange
        _fixture.CacheService
            .GetAsync<Weapon>(Arg.Any<string>())
            .ReturnsNull();

        _fixture.WeaponRepository
            .GetByIdAsync(Arg.Any<int>())
            .ReturnsNull();

        // Act
        var result = async () => await _fixture.WeaponService.GetByIdAsync(_fixture.Id);

        // Assert
        await result.Should().ThrowAsync<NullReferenceException>();
    }

    [Test]
    public void AddAsync_Should_ReturnTask_WhenWeaponIsValid()
    {
        // Act
        var result = _fixture.WeaponService.AddAsync(_fixture.Weapon);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void UpdateAsync_Should_ReturnTask_WhenWeaponIsValid()
    {
        // Act
        var result = _fixture.WeaponService.UpdateAsync(_fixture.Weapon);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void DeleteAsync_Should_ReturnTask_WhenWeaponIsValid()
    {
        // Act
        var result = _fixture.WeaponService.DeleteAsync(_fixture.Id);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void AddToCharacter_Should_ReturnVoid_WhenDataIsValid()
    {
        // Act
        var result = () => _fixture.WeaponService.AddToCharacterAsync(_fixture.Character, _fixture.Weapon);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void RemoveFromCharacter_Should_ReturnVoid_WhenWeaponExists()
    {
        // Act
        var result = () => _fixture.WeaponService.RemoveFromCharacterAsync(_fixture.Character, _fixture.Weapon);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void RemoveFromCharacter_Should_ThrowItemNotFoundException_WhenWeaponDoesNotExist()
    {
        // Act
        var result = async() => await _fixture.WeaponService.RemoveFromCharacterAsync(_fixture.Character, _fixture.Weapon);

        // Assert
        result.Should().ThrowAsync<ItemNotFoundException>();
    }
}
