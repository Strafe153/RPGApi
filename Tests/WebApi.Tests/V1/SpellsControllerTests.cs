using Domain.Dtos;
using Domain.Dtos.SpellDtos;
using Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using WebApi.Tests.V1.Fixtures;

namespace WebApi.Tests.V1;

[TestFixture]
public class SpellsControllerTests
{
    private SpellsControllerFixture _fixture = default!;

    [SetUp]
    public void SetUp()
    {
        _fixture = new SpellsControllerFixture();
        _fixture.MockObjectModelValidator(_fixture.SpellsController);
    }

    [TearDown]
    public void TearDown() => _fixture.SpellsController.ControllerContext = _fixture.MockControllerContext();

    [Test]
    public async Task Get_Should_ReturnActionResultOfPageDtoOfSpellReadDto_WhenPageParametersAreValid()
    {
        // Arrange
        _fixture.SpellService
            .GetAllAsync(Arg.Any<int>(), Arg.Any<int>())
            .Returns(_fixture.PaginatedList);

        // Act
        var result = await _fixture.SpellsController.Get(_fixture.PageParameters, _fixture.CancellationToken);
        var objectResult = result.Result.As<OkObjectResult>();
        var pageDto = objectResult.Value.As<PageDto<SpellReadDto>>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<ActionResult<PageDto<SpellReadDto>>>();
        objectResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        pageDto.Entities!.Count().Should().Be(_fixture.SpellsCount);
    }

    [Test]
    public async Task Get_Should_ReturnActionResultOfSpellReadDto_WhenSpellExists()
    {
        // Arrange
        _fixture.SpellService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Spell);

        // Act
        var result = await _fixture.SpellsController.Get(_fixture.Id, _fixture.CancellationToken);
        var objectResult = result.Result.As<OkObjectResult>();
        var readDto = objectResult.Value.As<SpellReadDto>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<ActionResult<SpellReadDto>>();
        objectResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        readDto.Should().NotBeNull();
    }

    [Test]
    public async Task Create_Should_ReturnActionResultOfSpellReadDto_WhenSpellBaseDtoIsValid()
    {
        // Act
        var result = await _fixture.SpellsController.Create(_fixture.SpellCreateDto);
        var objectResult = result.Result.As<CreatedAtActionResult>();
        var readDto = objectResult.Value.As<SpellReadDto>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<ActionResult<SpellReadDto>>();
        objectResult.StatusCode.Should().Be(StatusCodes.Status201Created);
        readDto.Should().NotBeNull();
    }

    [Test]
    public async Task Update_Should_ReturnNoContentResult_WhenSpellExistsAndSpellBaseDtoIsValid()
    {
        // Arrange
        _fixture.SpellService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Spell);

        // Act
        var result = await _fixture.SpellsController.Update(_fixture.Id, _fixture.SpellUpdateDto);
        var objectResult = result.As<NoContentResult>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        objectResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }

    [Test]
    public async Task Update_Should_ReturnNoContentResult_WhenSpellExistsAndPatchDocumentIsValid()
    {
        // Arrange
        _fixture.SpellService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Spell);

        // Act
        var result = await _fixture.SpellsController.Update(_fixture.Id, _fixture.PatchDocument);
        var objectResult = result.As<NoContentResult>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        objectResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }

    [Test]
    public async Task Update_Should_ReturnObjectResult_WhenSpellExistsAndPatchDocumentIsInvalid()
    {
        // Arrange
        _fixture.SpellService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Spell);

        _fixture.MockModelError(_fixture.SpellsController);

        // Act
        var result = await _fixture.SpellsController.Update(_fixture.Id, _fixture.PatchDocument);

        // Assert
        result.Should().NotBeNull().And.BeOfType<ObjectResult>();
    }

    [Test]
    public async Task Delete_Should_ReturnNoContentResult_WhenSpellExists()
    {
        // Arrange
        _fixture.SpellService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Spell);

        // Act
        var result = await _fixture.SpellsController.Delete(_fixture.Id);
        var objectResult = result.As<NoContentResult>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        objectResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }

    [Test]
    public async Task Hit_Should_ReturnNoContentResult_WhenHitDtoIsValid()
    {
        // Arrange
        _fixture.CharacterService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Character);

        _fixture.CharacterService
            .GetSpell(Arg.Any<Character>(), Arg.Any<int>())
            .Returns(_fixture.Spell);

        // Act
        var result = await _fixture.SpellsController.Hit(_fixture.HitDto);
        var objectResult = result.As<NoContentResult>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        objectResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }
}
