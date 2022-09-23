using Core.Dtos;
using Core.Dtos.CharacterDtos;
using Core.Entities;
using Core.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using WebApi.Tests.Fixtures;

namespace WebApi.Tests;

[TestFixture]
public class CharactersControllerTests
{
    private CharactersControllerFixture _fixture = default!;

    [OneTimeSetUp]
    public void SetUp()
    {
        _fixture = new CharactersControllerFixture();

        _fixture.MockControllerBaseUser();
        _fixture.MockObjectModelValidator(_fixture.CharactersController);
    }

    [TearDown]
    public void TearDown()
    {
        _fixture.CharactersController.ControllerContext = _fixture.MockControllerContext();
    }

    [Test]
    public async Task GetAsync_ValidPageParameters_ReturnsActionResultOfPageDtoOfCharacterReadDto()
    {
        _fixture.CharacterService
            .GetAllAsync(Arg.Any<int>(), Arg.Any<int>())
            .Returns(_fixture.PaginatedList);

        _fixture.PaginatedMapper
            .Map(Arg.Any<PaginatedList<Character>>())
            .Returns(_fixture.PageDto);

        // Act
        var result = await _fixture.CharactersController.GetAsync(_fixture.PageParameters);
        var pageDto = result.Result.As<OkObjectResult>().Value.As<PageDto<CharacterReadDto>>();

        result.Should().NotBeNull().And.BeOfType<ActionResult<PageDto<CharacterReadDto>>>();
        pageDto.Entities.Should().NotBeEmpty();
    }

    [Test]
    public async Task GetAsync_ExistingCharacter_ReturnsActionResultOfCharacterReadDto()
    {
        // Arrange
        _fixture.CharacterService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Character);

        _fixture.ReadMapper
            .Map(Arg.Any<Character>())
            .Returns(_fixture.CharacterReadDto);

        // Act
        var result = await _fixture.CharactersController.GetAsync(_fixture.Id);
        var readDto = result.Result.As<OkObjectResult>().Value.As<CharacterReadDto>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<ActionResult<CharacterReadDto>>();
        readDto.Should().NotBeNull();
    }
    
    [Test]
    public async Task CreateAsync_ValidDto_ReturnsActionResultOfCharacterReadDto()
    {
        // Arrange
        _fixture.CreateMapper
            .Map(Arg.Any<CharacterCreateDto>())
            .Returns(_fixture.Character);

        _fixture.ReadMapper
            .Map(Arg.Any<Character>())
            .Returns(_fixture.CharacterReadDto);

        // Act
        var result = await _fixture.CharactersController.CreateAsync(_fixture.CharacterCreateDto);
        var readDto = result.Result.As<CreatedAtActionResult>().Value.As<CharacterReadDto>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<ActionResult<CharacterReadDto>>();
        readDto.Should().NotBeNull();
    }

    [Test]
    public async Task UpdateAsync_ExistingCharacterValidDto_ReturnsNoContentResult()
    {
        // Arrange
        _fixture.CharacterService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Character);

        // Act
        var result = await _fixture.CharactersController.UpdateAsync(_fixture.Id, _fixture.CharacterUpdateDto);

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
    }

    [Test]
    public async Task UpdateAsync_ExistingCharacterValidPatchDocument_ReturnsNoContentResult()
    {
        // Arrange
        _fixture.CharacterService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Character);

        _fixture.UpdateMapper
            .Map(Arg.Any<Character>())
            .Returns(_fixture.CharacterUpdateDto);

        // Act
        var result = await _fixture.CharactersController.UpdateAsync(_fixture.Id, _fixture.PatchDocument);

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
    }

    [Test]
    public async Task UpdateAsync_ExistingCharacterInvalidPatchDocument_ReturnsObjectResult()
    {
        // Arrange
        _fixture.CharacterService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Character);

        _fixture.UpdateMapper
            .Map(Arg.Any<Character>())
            .Returns(_fixture.CharacterUpdateDto); 
        
        _fixture.MockModelError(_fixture.CharactersController);

        // Act
        var result = await _fixture.CharactersController.UpdateAsync(_fixture.Id, _fixture.PatchDocument);

        // Assert
        result.Should().NotBeNull().And.BeOfType<ObjectResult>();
    }

    [Test]
    public async Task DeleteAsync_ExistingCharacter_ReturnsNoContentResult()
    {
        // Arrange
        _fixture.CharacterService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Character);

        // Act
        var result = await _fixture.CharactersController.DeleteAsync(_fixture.Id);

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
    }

    [Test]
    public async Task AddWeaponAsync_ExistingCharacterExistingWeapon_ReturnsNoContentResult()
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

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
    }

    [Test]
    public async Task RemoveWeaponAsync_ExistingCharacterExistingWeapon_ReturnsNoContentResult()
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

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
    }

    [Test]
    public async Task AddSpellAsync_ExistingCharacterExistingSpell_ReturnsNoContentResult()
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

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
    }

    [Test]
    public async Task RemoveSpellAsync_ExistingCharacterExistingSpell_ReturnsNoContentResult()
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

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
    }

    [Test]
    public async Task AddMountAsync_ExistingCharacterExistingMount_ReturnsNoContentResult()
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

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
    }

    [Test]
    public async Task RemoveMountAsync_ExistingCharacterExistingMount_ReturnsNoContentResult()
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

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
    }
}
