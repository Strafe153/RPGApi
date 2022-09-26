using Core.Dtos;
using Core.Dtos.SpellDtos;
using Core.Entities;
using Core.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using WebApi.Tests.Fixtures;

namespace WebApi.Tests;

[TestFixture]
public class SpellsControllerTests
{
    private SpellsControllerFixture _fixture = default!;

    [OneTimeSetUp]
    public void SetUp()
    {
        _fixture = new();
        _fixture.MockObjectModelValidator(_fixture.SpellsController);
    }

    [TearDown]
    public void TearDown()
    {
        _fixture.SpellsController.ControllerContext = _fixture.MockControllerContext();
    }

    [Test]
    public async Task GetAsync_ValidPageParameters_ReturnsActionResultOfPageDtoOfSpellReadDto()
    {
        // Arrange
        _fixture.SpellService
            .GetAllAsync(Arg.Any<int>(), Arg.Any<int>())
            .Returns(_fixture.PaginatedList);

        _fixture.PaginatedMapper
            .Map(Arg.Any<PaginatedList<Spell>>())
            .Returns(_fixture.PageDto);

        // Act
        var result = await _fixture.SpellsController.GetAsync(_fixture.PageParameters);
        var objectResult = result.Result.As<OkObjectResult>();
        var pageDto = objectResult.Value.As<PageDto<SpellReadDto>>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<ActionResult<PageDto<SpellReadDto>>>();
        objectResult.StatusCode.Should().Be(200);
        pageDto.Entities.Should().NotBeEmpty();
    }

    [Test]
    public async Task GetAsync_ExistingSpell_ReturnsActionResultOfSpellReadDto()
    {
        // Arrange
        _fixture.SpellService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Spell);

        _fixture.ReadMapper
            .Map(Arg.Any<Spell>())
            .Returns(_fixture.SpellReadDto);

        // Act
        var result = await _fixture.SpellsController.GetAsync(_fixture.Id);
        var objectResult = result.Result.As<OkObjectResult>();
        var readDto = objectResult.Value.As<SpellReadDto>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<ActionResult<SpellReadDto>>();
        objectResult.StatusCode.Should().Be(200);
        readDto.Should().NotBeNull();
    }

    [Test]
    public async Task CreateAsync_ValidDto_ReturnsActionResultOfSpellReadDto()
    {
        // Arrange
        _fixture.CreateMapper
            .Map(Arg.Any<SpellBaseDto>())
            .Returns(_fixture.Spell);

        _fixture.ReadMapper
            .Map(Arg.Any<Spell>())
            .Returns(_fixture.SpellReadDto);

        // Act
        var result = await _fixture.SpellsController.CreateAsync(_fixture.SpellBaseDto);
        var objectResult = result.Result.As<CreatedAtActionResult>();
        var readDto = objectResult.Value.As<SpellReadDto>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<ActionResult<SpellReadDto>>();
        objectResult.StatusCode.Should().Be(201);
        readDto.Should().NotBeNull();
    }

    [Test]
    public async Task UpdateAsync_ExistingSpellValidDto_ReturnsNoContentResult()
    {
        // Arrange
        _fixture.SpellService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Spell);

        // Act
        var result = await _fixture.SpellsController.UpdateAsync(_fixture.Id, _fixture.SpellBaseDto);
        var objectResult = result.As<NoContentResult>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        objectResult.StatusCode.Should().Be(204);
    }

    [Test]
    public async Task UpdateAsync_ExistingSpellValidPatchDocument_ReturnsNoContentResult()
    {
        // Arrange
        _fixture.SpellService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Spell);

        _fixture.UpdateMapper
            .Map(Arg.Any<Spell>())
            .Returns(_fixture.SpellBaseDto);

        // Act
        var result = await _fixture.SpellsController.UpdateAsync(_fixture.Id, _fixture.PatchDocument);
        var objectResult = result.As<NoContentResult>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        objectResult.StatusCode.Should().Be(204);
    }

    [Test]
    public async Task UpdateAsync_ExistingSpellInvalidPatchDocument_ReturnsObjectResult()
    {
        // Arrange
        _fixture.SpellService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Spell);

        _fixture.UpdateMapper
            .Map(Arg.Any<Spell>())
            .Returns(_fixture.SpellBaseDto);

        _fixture.MockModelError(_fixture.SpellsController);

        // Act
        var result = await _fixture.SpellsController.UpdateAsync(_fixture.Id, _fixture.PatchDocument);

        // Assert
        result.Should().NotBeNull().And.BeOfType<ObjectResult>();
    }

    [Test]
    public async Task DeleteAsync_ExistingSpell_ReturnsNoContentResult()
    {
        // Arrange
        _fixture.SpellService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Spell);

        // Act
        var result = await _fixture.SpellsController.DeleteAsync(_fixture.Id);
        var objectResult = result.As<NoContentResult>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        objectResult.StatusCode.Should().Be(204);
    }

    [Test]
    public async Task HitAsync_ValidDto_ReturnsNoContentResult()
    {
        // Arrange
        _fixture.CharacterService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Character);

        _fixture.CharacterService
            .GetSpell(Arg.Any<Character>(), Arg.Any<int>())
            .Returns(_fixture.Spell);

        // Act
        var result = await _fixture.SpellsController.HitAsync(_fixture.HitDto);
        var objectResult = result.As<NoContentResult>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        objectResult.StatusCode.Should().Be(204);
    }
}
