using Application.Dtos;
using Application.Dtos.CharactersDtos;
using Domain.Enums;
using Domain.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using WebApi.Tests.V1.Fixtures;

namespace WebApi.Tests.V1;

[TestFixture]
public class CharactersControllerTests
{
	private CharactersControllerFixture _fixture = default!;

	[SetUp]
	public void SetUp() => _fixture = new();

	[Test]
	public async Task Get_Should_ReturnActionResultOfPageDtoOfCharacterReadDto_WhenDataIsValid()
    {
		// Arrange
		List<CharacterReadDto> characters = [
            new(
                1,
                "Ren",
                CharacterRace.Human,
                100,
                1,
                [],
                [],
                []),
            new(
                2,
                "Eltolinde",
                CharacterRace.Elf,
                100,
                1,
                [],
                [],
                [])
        ];

        PageDto<CharacterReadDto> pageDto = new(1, 1, 2, 2, false, false, characters);

        PageParameters pageParameters = new()
        {
            PageNumber = 1,
            PageSize = 2
        };

        _fixture.CharactersService
            .Setup(r => r.GetAllAsync(It.IsAny<PageParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pageDto);

        // Act
        var sut = _fixture.CreateSut();
		var result = await sut.Get(pageParameters, new CancellationToken());
		var objectResult = result.Result as OkObjectResult;
		var resultPageDto = objectResult?.Value as PageDto<CharacterReadDto>;

		// Assert
        Assert.AreEqual(StatusCodes.Status200OK, objectResult?.StatusCode);
        Assert.AreEqual("Ren", resultPageDto?.Entities.First().Name);
        Assert.AreEqual(2, resultPageDto?.Entities.Last().Id);
	}

	[Test]
	public async Task Get_Should_ReturnCreatedAtActionResultOfCharacterReadDto_WhenCharacterExists()
    {
        // Arrange
        var id = 1;

        CharacterReadDto character = new(
            id,
            "Ren",
            CharacterRace.Human,
            100,
            1,
            [],
            [],
            []);

        _fixture.CharactersService
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(character);

        // Act
        var sut = _fixture.CreateSut();
		var result = await sut.Get(1, new CancellationToken());
		var objectResult = result.Result as OkObjectResult;
		var readDto = objectResult?.Value as CharacterReadDto;

		// Assert
		Assert.AreEqual(StatusCodes.Status200OK, objectResult?.StatusCode);
        Assert.AreEqual(1, readDto?.Id);
	}

	[Test]
	public async Task Create_Should_ReturnActionResultOfCharacterReadDto_WhenCharacterCreateDtoIsValid()
    {
		// Arrange
        CharacterReadDto character = new(
            1,
            "Ren",
            CharacterRace.Human,
            100,
            1,
            [],
            [],
            []);

        CharacterCreateDto createDto = new("Ren", 1, CharacterRace.Human);

        _fixture.CharactersService
            .Setup(r => r.AddAsync(createDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(character);

        // Act
        var sut = _fixture.CreateSut();
		var result = await sut.Create(createDto, new CancellationToken());
		var objectResult = result.Result as CreatedAtActionResult;
		var readDto = objectResult?.Value as CharacterReadDto;

		// Assert
		Assert.AreEqual(StatusCodes.Status201Created, objectResult?.StatusCode);
        Assert.AreEqual(CharacterRace.Human, readDto?.Race);
	}

	[Test]
	public async Task Update_Should_ReturnNoContentResult_WhenCharacterExistsAndDtoIsValid()
    {
        // Arrange
        CharacterUpdateDto updateDto = new("Eltolinde", CharacterRace.Elf);

        // Act
        var sut = _fixture.CreateSut();
		var result = await sut.Update(1, updateDto, new CancellationToken());
		var objectResult = result as NoContentResult;

        // Assert
        Assert.AreEqual(StatusCodes.Status204NoContent, objectResult?.StatusCode);
	}

	[Test]
	public async Task Update_Should_ReturnNoContentResult_WhenCharacterExistsAndPatchDocumentIsValid()
    {
		// Arrange
        JsonPatchDocument<CharacterUpdateDto> patchDocument = new();

        _fixture.CharactersService
            .Setup(r => r.PatchAsync(
                1,
                It.IsAny<JsonPatchDocument<CharacterUpdateDto>>(),
                It.IsAny<Func<object, bool>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var sut = _fixture.CreateSut();
		var result = await sut.Update(1, patchDocument, new CancellationToken());
		var objectResult = result as NoContentResult;

		// Assert
		Assert.AreEqual(StatusCodes.Status204NoContent, objectResult?.StatusCode);
	}

	[Test]
	public async Task Delete_Should_ReturnNoContentResult_WhenCharacterExists()
    {
        // Act
        var sut = _fixture.CreateSut();
		var result = await sut.Delete(1, new CancellationToken());
		var objectResult = result as NoContentResult;

		// Assert
		Assert.AreEqual(StatusCodes.Status204NoContent, objectResult?.StatusCode);
	}

	[Test]
	[TestCaseSource(nameof(ManageItemTestData))]
	public async Task ManageItem_Should_ReturnNoContentResult_WhenCharacterAndItemExist(ManageItemDto itemDto)
    {
        // Act
        var sut = _fixture.CreateSut();
		var result = await sut.ManageItem(itemDto, new CancellationToken());
		var objectResult = result as NoContentResult;

		// Assert
		Assert.AreEqual(StatusCodes.Status204NoContent, objectResult?.StatusCode);
	}

	[Test]
	[TestCaseSource(nameof(HitTestData))]
	public async Task Hit_Should_ReturnNoContentResult_WhenCharacterAndItemExist(HitDto hitDto)
    {
        // Act
        var sut = _fixture.CreateSut();
		var result = await sut.Hit(hitDto, new CancellationToken());
		var objectResult = result as NoContentResult;

		// Assert
		Assert.AreEqual(StatusCodes.Status204NoContent, objectResult?.StatusCode);
	}

	private static IEnumerable<ManageItemDto> ManageItemTestData()
	{
		yield return new(1, 1, ItemType.Weapon, ManageItemOperation.Add);
		yield return new(1, 1, ItemType.Weapon, ManageItemOperation.Remove);
		yield return new(1, 1, ItemType.Spell, ManageItemOperation.Add);
		yield return new(1, 1, ItemType.Spell, ManageItemOperation.Remove);
		yield return new(1, 1, ItemType.Mount, ManageItemOperation.Add);
		yield return new(1, 1, ItemType.Mount, ManageItemOperation.Remove);
	}

	private static IEnumerable<HitDto> HitTestData()
	{
		yield return new(1, 1, 1, HitType.Weapon);
		yield return new(1, 1, 1, HitType.Spell);
	}
}
