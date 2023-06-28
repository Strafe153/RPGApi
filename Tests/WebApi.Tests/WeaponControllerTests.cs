using Core.Dtos;
using Core.Dtos.WeaponDtos;
using Core.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using WebApi.Tests.Fixtures;

namespace WebApi.Tests;

[TestFixture]
public class WeaponControllerTests
{
    private WeaponsControllerFixture _fixture = default!;

    [OneTimeSetUp]
    public void SetUp()
    {
        _fixture = new WeaponsControllerFixture();
        _fixture.MockObjectModelValidator(_fixture.WeaponsController);
    }

    [TearDown]
    public void TearDown()
    {
        _fixture.WeaponsController.ControllerContext = _fixture.MockControllerContext();
    }

    [Test]
    public async Task GetAsync_Should_ReturnActionResultOfPageDtoOfWeaponReadDto_WhenPageParametersAreValid()
    {
        // Arrange
        _fixture.WeaponService
            .GetAllAsync(Arg.Any<int>(), Arg.Any<int>())
            .Returns(_fixture.PaginatedList);

        // Act
        var result = await _fixture.WeaponsController.GetAsync(_fixture.PageParameters, _fixture.CancellationToken);
        var objectResult = result.Result.As<OkObjectResult>();
        var pageDto = objectResult.Value.As<PageDto<WeaponReadDto>>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<ActionResult<PageDto<WeaponReadDto>>>();
        objectResult.StatusCode.Should().Be(200);
        pageDto.Entities!.Count().Should().Be(_fixture.WeaponsCount);
    }

    [Test]
    public async Task GetAsync_Should_ReturnActionResultOfWeaponReadDto_WhenWeaponExists()
    {
        // Arrange
        _fixture.WeaponService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Weapon);

        // Act
        var result = await _fixture.WeaponsController.GetAsync(_fixture.Id, _fixture.CancellationToken);
        var objectResult = result.Result.As<OkObjectResult>();
        var readDto = objectResult.Value.As<WeaponReadDto>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<ActionResult<WeaponReadDto>>();
        objectResult.StatusCode.Should().Be(200);
        readDto.Should().NotBeNull();
    }

    [Test]
    public async Task CreateAsync_Should_ReturnActionResultOfWeaponReadDto_WhenWeaponBaseDtoIsValid()
    {
        // Act
        var result = await _fixture.WeaponsController.CreateAsync(_fixture.WeaponBaseDto);
        var objectResult = result.Result.As<CreatedAtActionResult>();
        var readDto = objectResult.Value.As<WeaponReadDto>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<ActionResult<WeaponReadDto>>();
        objectResult.StatusCode.Should().Be(201);
        readDto.Should().NotBeNull();
    }

    [Test]
    public async Task UpdateAsync_Should_ReturnNoContentResult_WhenWeaponBaseDtoIsValid()
    {
        // Arrange
        _fixture.WeaponService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Weapon);

        // Act
        var result = await _fixture.WeaponsController.UpdateAsync(_fixture.Id, _fixture.WeaponBaseDto);
        var objectResult = result.As<NoContentResult>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        objectResult.StatusCode.Should().Be(204);
    }

    [Test]
    public async Task UpdateAsync_Should_ReturnNoContentResult_WhenWeaponExistsAndPatchDocumentIsValid()
    {
        // Arrange
        _fixture.WeaponService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Weapon);

        // Act
        var result = await _fixture.WeaponsController.UpdateAsync(_fixture.Id, _fixture.PatchDocument);
        var objectResult = result.As<NoContentResult>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        objectResult.StatusCode.Should().Be(204);
    }

    [Test]
    public async Task UpdateAsync_Should_ReturnObjectResult_WhenWeaponExistsAndPatchDocumentIsInvalid()
    {
        // Arrange
        _fixture.WeaponService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Weapon);

        _fixture.MockModelError(_fixture.WeaponsController);

        // Act
        var result = await _fixture.WeaponsController.UpdateAsync(_fixture.Id, _fixture.PatchDocument);

        // Assert
        result.Should().NotBeNull().And.BeOfType<ObjectResult>();
    }

    [Test]
    public async Task DeleteAsync_Should_ReturnNoContentResult_WhenWeaponExists()
    {
        // Arrange
        _fixture.WeaponService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Weapon);

        // Act
        var result = await _fixture.WeaponsController.DeleteAsync(_fixture.Id);
        var objectResult = result.As<NoContentResult>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        objectResult.StatusCode.Should().Be(204);
    }

    [Test]
    public async Task HitAsync_Should_ReturnNoContentResult_WhenHitDtoIsValid()
    {
        // Arrange
        _fixture.CharacterService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Character);

        _fixture.CharacterService
            .GetWeapon(Arg.Any<Character>(), Arg.Any<int>())
            .Returns(_fixture.Weapon);

        // Act
        var result = await _fixture.WeaponsController.HitAsync(_fixture.HitDto);
        var objectResult = result.As<NoContentResult>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        objectResult.StatusCode.Should().Be(204);
    }
}
