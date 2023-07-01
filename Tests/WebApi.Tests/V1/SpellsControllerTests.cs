using Core.Dtos;
using Core.Dtos.SpellDtos;
using Core.Entities;
using Core.Shared;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using WebApi.Tests.V1.Fixtures;

namespace WebApi.Tests.V1;

[TestFixture]
public class SpellsControllerTests
{
    private SpellsControllerFixture _fixture = default!;

    [OneTimeSetUp]
    public void SetUp()
    {
        _fixture = new SpellsControllerFixture();
        _fixture.MockObjectModelValidator(_fixture.SpellsController);
    }

    [TearDown]
    public void TearDown()
    {
        _fixture.SpellsController.ControllerContext = _fixture.MockControllerContext();
    }

    [Test]
    public async Task GetAsync_Should_ReturnActionResultOfPageDtoOfSpellReadDto_WhenPageParametersAreValid()
    {
        // Arrange
        _fixture.SpellService
            .GetAllAsync(Arg.Any<int>(), Arg.Any<int>())
            .Returns(_fixture.PaginatedList);

        // Act
        var result = await _fixture.SpellsController.GetAsync(_fixture.PageParameters, _fixture.CancellationToken);
        var objectResult = result.Result.As<OkObjectResult>();
        var pageDto = objectResult.Value.As<PageDto<SpellReadDto>>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<ActionResult<PageDto<SpellReadDto>>>();
        objectResult.StatusCode.Should().Be(200);
        pageDto.Entities!.Count().Should().Be(_fixture.SpellsCount);
    }

    [Test]
    public async Task GetAsync_Should_ReturnActionResultOfSpellReadDto_WhenSpellExists()
    {
        // Arrange
        _fixture.SpellService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Spell);

        // Act
        var result = await _fixture.SpellsController.GetAsync(_fixture.Id, _fixture.CancellationToken);
        var objectResult = result.Result.As<OkObjectResult>();
        var readDto = objectResult.Value.As<SpellReadDto>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<ActionResult<SpellReadDto>>();
        objectResult.StatusCode.Should().Be(200);
        readDto.Should().NotBeNull();
    }

    [Test]
    public async Task CreateAsync_Should_ReturnActionResultOfSpellReadDto_WhenSpellBaseDtoIsValid()
    {
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
    public async Task UpdateAsync_Should_ReturnNoContentResult_WhenSpellExistsAndSpellBaseDtoIsValid()
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
    public async Task UpdateAsync_Should_ReturnNoContentResult_WhenSpellExistsAndPatchDocumentIsValid()
    {
        // Arrange
        _fixture.SpellService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Spell);

        // Act
        var result = await _fixture.SpellsController.UpdateAsync(_fixture.Id, _fixture.PatchDocument);
        var objectResult = result.As<NoContentResult>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        objectResult.StatusCode.Should().Be(204);
    }

    [Test]
    public async Task UpdateAsync_Should_ReturnObjectResult_WhenSpellExistsAndPatchDocumentIsInvalid()
    {
        // Arrange
        _fixture.SpellService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Spell);

        _fixture.MockModelError(_fixture.SpellsController);

        // Act
        var result = await _fixture.SpellsController.UpdateAsync(_fixture.Id, _fixture.PatchDocument);

        // Assert
        result.Should().NotBeNull().And.BeOfType<ObjectResult>();
    }

    [Test]
    public async Task DeleteAsync_Should_ReturnNoContentResult_WhenSpellExists()
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
    public async Task HitAsync_Should_ReturnNoContentResult_WhenHitDtoIsValid()
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
