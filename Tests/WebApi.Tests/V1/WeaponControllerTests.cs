using Application.Dtos;
using Application.Dtos.WeaponDtos;
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
public class WeaponControllerTests
{
	private WeaponsControllerFixture _fixture = default!;

	[SetUp]
	public void SetUp() => _fixture = new();

	[Test]
	public async Task Get_Should_ReturnActionResultOfPageDtoOfWeaponReadDto_WhenDataIsValid()
    {
        // Arrange
        List<WeaponReadDto> weapons = [
            new(1, "Iron Bow", WeaponType.Bow, 21, []),
            new(2, "Rebel Knife", WeaponType.Dagger, 87, [])
        ];

        PageDto<WeaponReadDto> pageDto = new(1, 1, 2, 2, false, false, weapons);

        PageParameters pageParameters = new()
        {
            PageNumber = 1,
            PageSize = 2
        };

        _fixture.WeaponsService
            .Setup(r => r.GetAllAsync(It.IsAny<PageParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pageDto);

        // Act
        var sut = _fixture.CreateSut();
		var result = await sut.Get(pageParameters, new CancellationToken());
		var objectResult = result.Result as OkObjectResult;
		var resultPageDto = objectResult?.Value as PageDto<WeaponReadDto>;

		// Assert
		Assert.AreEqual(StatusCodes.Status200OK, objectResult?.StatusCode);
        Assert.AreEqual(WeaponType.Bow, resultPageDto?.Entities.First().Type);
        Assert.AreEqual(2, resultPageDto?.Entities.Last().Id);
	}

	[Test]
	public async Task Get_Should_ReturnActionResultOfWeaponReadDto_WhenWeaponExists()
    {
        // Arrange
        var id = 1;
        WeaponReadDto weapon = new(id, "Rebel Knife", WeaponType.Dagger, 87, []);

        _fixture.WeaponsService
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(weapon);

        // Act
        var sut = _fixture.CreateSut();
		var result = await sut.Get(1, new CancellationToken());
		var objectResult = result.Result as OkObjectResult;
		var readDto = objectResult?.Value as WeaponReadDto;

		// Assert
		Assert.AreEqual(StatusCodes.Status200OK, objectResult?.StatusCode);
        Assert.AreEqual(87, readDto?.Damage);
	}

	[Test]
	public async Task Create_Should_ReturnCreatedAtActionResultOfWeaponReadDto_WhenDataIsValid()
	{
		// Arrange
        WeaponReadDto weapon = new(1, "Rebel Knife", WeaponType.Dagger, 87, []);
        WeaponCreateDto createDto = new("Rebel Knife", WeaponType.Dagger, 87);

        _fixture.WeaponsService
            .Setup(r => r.AddAsync(createDto))
            .ReturnsAsync(weapon);

        // Act
        var sut = _fixture.CreateSut();
		var result = await sut.Create(createDto);
		var objectResult = result.Result as CreatedAtActionResult;
		var readDto = objectResult?.Value as WeaponReadDto;

		// Assert
		Assert.AreEqual(StatusCodes.Status201Created, objectResult?.StatusCode);
        Assert.AreEqual("Rebel Knife", readDto?.Name);
	}

	[Test]
	public async Task Update_Should_ReturnNoContentResult_WhenWeaponExistsAndDtoIsValid()
    {
        // Arrange
        WeaponUpdateDto updateDto = new("Rebel Knife", WeaponType.Dagger, 87);

        // Act
        var sut = _fixture.CreateSut();
        var result = await sut.Update(1, updateDto, new CancellationToken());
		var objectResult = result as NoContentResult;

		// Assert
		Assert.AreEqual(StatusCodes.Status204NoContent, objectResult?.StatusCode);
	}

	[Test]
	public async Task Update_Should_ReturnNoContentResult_WhenWeaponExistsAndPatchDocumentIsValid()
	{
        // Arrange
        JsonPatchDocument<WeaponUpdateDto> patchDocument = new();

        _fixture.WeaponsService
            .Setup(r => r.PatchAsync(
                It.IsAny<int>(),
                It.IsAny<JsonPatchDocument<WeaponUpdateDto>>(),
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
	public async Task Delete_Should_ReturnNoContentResult_WhenWeaponExists()
    {
        // Act
        var sut = _fixture.CreateSut();
		var result = await sut.Delete(1, new CancellationToken());
		var objectResult = result as NoContentResult;

		// Assert
		Assert.AreEqual(StatusCodes.Status204NoContent, objectResult?.StatusCode);
	}
}
