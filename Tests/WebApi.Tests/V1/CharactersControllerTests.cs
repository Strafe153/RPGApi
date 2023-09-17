using Core.Dtos;
using Core.Dtos.CharacterDtos;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using WebApi.Tests.V1.Fixtures;

namespace WebApi.Tests.V1;

[TestFixture]
public class CharactersControllerTests
{
    private CharactersControllerFixture _fixture = default!;

    [SetUp]
    public void SetUp()
    {
        _fixture = new CharactersControllerFixture();
        _fixture.MockObjectModelValidator(_fixture.CharactersController);
    }

    [TearDown]
    public void TearDown()
    {
        _fixture.CharactersController.ControllerContext = _fixture.MockControllerContext();
    }

    [Test]
    public async Task GetAsync_Should_ReturnActionResultOfPageDtoOfCharacterReadDto_WhenPageParametersAreValid()
    {
        // Arrange
        _fixture.CharacterService
            .GetAllAsync(Arg.Any<int>(), Arg.Any<int>())
            .Returns(_fixture.PaginatedList);

        // Act
        var result = await _fixture.CharactersController.GetAsync(_fixture.PageParameters, _fixture.CancellationToken);
        var objectResult = result.Result.As<OkObjectResult>();
        var pageDto = objectResult.Value.As<PageDto<CharacterReadDto>>();

        result.Should().NotBeNull().And.BeOfType<ActionResult<PageDto<CharacterReadDto>>>();
        objectResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        pageDto.Entities!.Count().Should().Be(_fixture.CharactersCount);
    }

    [Test]
    public async Task GetAsync_Should_ReturnActionResultOfCharacterReadDto_WhenCharacterExists()
    {
        // Arrange
        _fixture.CharacterService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Character);

        // Act
        var result = await _fixture.CharactersController.GetAsync(_fixture.Id, _fixture.CancellationToken);
        var objectResult = result.Result.As<OkObjectResult>();
        var readDto = objectResult.Value.As<CharacterReadDto>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<ActionResult<CharacterReadDto>>();
        objectResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        readDto.Should().NotBeNull();
    }

    [Test]
    public async Task CreateAsync_Should_ReturnActionResultOfCharacterReadDto_WhenCharacterCreateDtoIsValid()
    {
        // Act
        var result = await _fixture.CharactersController.CreateAsync(_fixture.CharacterCreateDto);
        var objectResult = result.Result.As<CreatedAtActionResult>();
        var readDto = objectResult.Value.As<CharacterReadDto>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<ActionResult<CharacterReadDto>>();
        objectResult.StatusCode.Should().Be(StatusCodes.Status201Created);
        readDto.Should().NotBeNull();
    }

    [Test]
    public async Task UpdateAsync_Should_ReturnNoContentResult_WhenCharacterExistsAndCharacterUpdateDtoIsValid()
    {
        // Arrange
        _fixture.CharacterService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Character);

        // Act
        var result = await _fixture.CharactersController.UpdateAsync(_fixture.Id, _fixture.CharacterUpdateDto);
        var objectResult = result.As<NoContentResult>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        objectResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }

    [Test]
    public async Task UpdateAsync_Should_ReturnNoContentResult_WhenCharacterExistsAndPatchDocumentIsValid()
    {
        // Arrange
        _fixture.CharacterService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Character);

        // Act
        var result = await _fixture.CharactersController.UpdateAsync(_fixture.Id, _fixture.PatchDocument);
        var objectResult = result.As<NoContentResult>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        objectResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }

    [Test]
    public async Task UpdateAsync_Should_ReturnObjectResult_WhenCharacterExistsAndPatchDocumentIsInvalid()
    {
        // Arrange
        _fixture.CharacterService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Character);

        _fixture.MockModelError(_fixture.CharactersController);

        // Act
        var result = await _fixture.CharactersController.UpdateAsync(_fixture.Id, _fixture.PatchDocument);

        // Assert
        result.Should().NotBeNull().And.BeOfType<ObjectResult>();
    }

    [Test]
    public async Task DeleteAsync_Should_ReturnNoContentResult_WhenCharacterExists()
    {
        // Arrange
        _fixture.CharacterService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Character);

        // Act
        var result = await _fixture.CharactersController.DeleteAsync(_fixture.Id);
        var objectResult = result.As<NoContentResult>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        objectResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }

    [Test]
    public async Task AddWeaponAsync_Should_ReturnNoContentResult_WhenCharacterAndWeaponExist()
    {
        // Arrange
        _fixture.CharacterService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Character);

        _fixture.WeaponService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Weapon);

        // Act
        var result = await _fixture.CharactersController.AddWeaponAsync(_fixture.ItemDto);
        var objectResult = result.As<NoContentResult>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        objectResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }

    [Test]
    public async Task RemoveWeaponAsync_Should_ReturnNoContentResult_WhenCharacterAndWeaponExist()
    {
        // Arrange
        _fixture.CharacterService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Character);

        _fixture.WeaponService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Weapon);

        // Act
        var result = await _fixture.CharactersController.RemoveWeaponAsync(_fixture.ItemDto);
        var objectResult = result.As<NoContentResult>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        objectResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }

    [Test]
    public async Task AddSpellAsync_Should_ReturnNoContentResult_WhenCharacterAndSpellExist()
    {
        // Arrange
        _fixture.CharacterService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Character);

        _fixture.SpellService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Spell);

        // Act
        var result = await _fixture.CharactersController.AddSpellAsync(_fixture.ItemDto);
        var objectResult = result.As<NoContentResult>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        objectResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }

    [Test]
    public async Task RemoveSpellAsync_Should_ReturnNoContentResult_WhenCharacterAndSpellExist()
    {
        // Arrange
        _fixture.CharacterService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Character);

        _fixture.SpellService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Spell);

        // Act
        var result = await _fixture.CharactersController.RemoveSpellAsync(_fixture.ItemDto);
        var objectResult = result.As<NoContentResult>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        objectResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }

    [Test]
    public async Task AddMountAsync_Should_ReturnNoContentResult_WhenCharacterAndMountExist()
    {
        // Arrange
        _fixture.CharacterService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Character);

        _fixture.MountService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Mount);

        // Act
        var result = await _fixture.CharactersController.AddMountAsync(_fixture.ItemDto);
        var objectResult = result.As<NoContentResult>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        objectResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }

    [Test]
    public async Task RemoveMountAsync_Should_ReturnNoContentResult_WhenCharacterAndMountExist()
    {
        // Arrange
        _fixture.CharacterService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Character);

        _fixture.MountService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Mount);

        // Act
        var result = await _fixture.CharactersController.RemoveMountAsync(_fixture.ItemDto);
        var objectResult = result.As<NoContentResult>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        objectResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }
}
