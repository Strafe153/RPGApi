using Application.Dtos;
using Application.Dtos.SpellDtos;
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
public class SpellsControllerTests
{
	private SpellsControllerFixture _fixture = default!;

	[SetUp]
	public void SetUp() => _fixture = new();

	[Test]
	public async Task Get_Should_ReturnActionResultOfPageDtoOfSpellReadDto_WhenDataIsValid()
    {
        // Arrange
        List<SpellReadDto> spells = [
            new(1, "Splash", SpellType.Water, 8, []),
            new(2, "Lightning", SpellType.Thunder, 44, [])
        ];

        PageDto<SpellReadDto> pageDto = new(1, 1, 2, 2, false, false, spells);

        PageParameters pageParameters = new()
        {
            PageNumber = 1,
            PageSize = 2
        };

        _fixture.SpellsService
            .Setup(r => r.GetAllAsync(It.IsAny<PageParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pageDto);

        // Act
        var sut = _fixture.CreateSut();
		var result = await sut.Get(pageParameters, new CancellationToken());
		var objectResult = result.Result as OkObjectResult;
		var resultPageDto = objectResult?.Value as PageDto<SpellReadDto>;

		// Assert
		Assert.AreEqual(StatusCodes.Status200OK, objectResult?.StatusCode);
        Assert.AreEqual("Splash", resultPageDto?.Entities.First().Name);
        Assert.AreEqual(44, resultPageDto?.Entities.Last().Damage);
	}

	[Test]
	public async Task Get_Should_ReturnActionResultOfSpellReadDto_WhenSpellExists()
    {
        // Arrange
        var id = 1;
        SpellReadDto spell = new(id, "Splash", SpellType.Water, 8, []);

        _fixture.SpellsService
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(spell);

        // Act
        var sut = _fixture.CreateSut();
		var result = await sut.Get(1, new CancellationToken());
		var objectResult = result.Result as OkObjectResult;
		var readDto = objectResult?.Value as SpellReadDto;

		// Assert
		Assert.AreEqual(StatusCodes.Status200OK, objectResult?.StatusCode);
        Assert.AreEqual(SpellType.Water, readDto?.Type);
	}

	[Test]
	public async Task Create_Should_ReturnCreatedAtActionResultOfSpellReadDto_WhenDtoIsValid()
    {
		// Arrange
        SpellReadDto spell = new(1, "Splash", SpellType.Water, 8, []);
        SpellCreateDto createDto = new("Splash", SpellType.Water, 8);

        _fixture.SpellsService
            .Setup(r => r.AddAsync(createDto))
            .ReturnsAsync(spell);

        // Act
        var sut = _fixture.CreateSut();
		var result = await sut.Create(createDto);
		var objectResult = result.Result as CreatedAtActionResult;
		var readDto = objectResult?.Value as SpellReadDto;

		// Assert
		Assert.AreEqual(StatusCodes.Status201Created, objectResult?.StatusCode);
        Assert.AreEqual("Splash", readDto?.Name);
	}

	[Test]
	public async Task Update_Should_ReturnNoContentResult_WhenSpellExistsAndDtoIsValid()
    {
        // Arrange
        SpellUpdateDto updateDto = new("Splash", SpellType.Water, 8);

        // Act
        var sut = _fixture.CreateSut();
		var result = await sut.Update(1, updateDto, new CancellationToken());
		var objectResult = result as NoContentResult;

		// Assert
		Assert.AreEqual(StatusCodes.Status204NoContent, objectResult?.StatusCode);
	}

	[Test]
	public async Task Update_Should_ReturnNoContentResult_WhenSpellExistsAndPatchDocumentIsValid()
	{
		// Arrange
		JsonPatchDocument<SpellUpdateDto> patchDocument = new();

        _fixture.SpellsService
            .Setup(r => r.PatchAsync(
                It.IsAny<int>(),
                It.IsAny<JsonPatchDocument<SpellUpdateDto>>(),
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
	public async Task Delete_Should_ReturnNoContentResult_WhenSpellExists()
    {
        // Act
        var sut = _fixture.CreateSut();
		var result = await sut.Delete(1, new CancellationToken());
		var objectResult = result as NoContentResult;

		// Assert
		Assert.AreEqual(StatusCodes.Status204NoContent, objectResult?.StatusCode);
	}
}
