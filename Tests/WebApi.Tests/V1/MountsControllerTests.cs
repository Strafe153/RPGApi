using Application.Dtos;
using Application.Dtos.MountDtos;
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
public class MountsControllerTests
{
	private MountsControllerFixture _fixture = default!;

	[SetUp]
	public void SetUp() => _fixture = new();

	[Test]
	public async Task Get_Should_ReturnActionResultOfPageDtoOfMountReadDto_WhenDataIsValid()
    {
        // Arrange
        List<MountReadDto> mounts = [
            new(1, "Max Elephant", MountType.Elephant, 5, []),
            new(2, "Bigby Wolf", MountType.Wolf, 69, [])
        ];

        PageDto<MountReadDto> pageDto = new(1, 1, 2, 2, false, false, mounts);

        PageParameters pageParameters = new()
        {
            PageNumber = 1,
            PageSize = 2
        };

        _fixture.MountsService
            .Setup(r => r.GetAllAsync(It.IsAny<PageParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pageDto);

        // Act
        var sut = _fixture.CreateSut();
		var result = await sut.Get(pageParameters, new CancellationToken());
		var objectResult = result.Result as OkObjectResult;
		var resultPageDto = objectResult?.Value as PageDto<MountReadDto>;

		// Assert
		Assert.AreEqual(StatusCodes.Status200OK, objectResult?.StatusCode);
        Assert.AreEqual(1, resultPageDto?.Entities.First().Id);
        Assert.AreEqual("Bigby Wolf", resultPageDto?.Entities.Last().Name);
	}

	[Test]
	public async Task Get_Should_ReturnActionREsultOfMountReadDto_WhenMountExists()
    {
        // Arrange
        var id = 1;
        MountReadDto mount = new(id, "Max Elephant", MountType.Elephant, 5, []);

        _fixture.MountsService
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mount);

        // Act
        var sut = _fixture.CreateSut();
		var result = await sut.Get(1, new CancellationToken());
		var objectResult = result.Result as OkObjectResult;
		var readDto = objectResult?.Value as MountReadDto;

		// Assert
		Assert.AreEqual(StatusCodes.Status200OK, objectResult?.StatusCode);
        Assert.AreEqual("Max Elephant", readDto?.Name);
	}

	[Test]
	public async Task Create_Should_ReturnCreatedAtActionResultOfMountReadDto_WhenDtoIsValid()
    {
        // Arrange
        MountReadDto mount = new(1, "Max Elephant", MountType.Elephant, 5, []);
        MountCreateDto createDto = new("Max Elephant", MountType.Elephant, 5);

        _fixture.MountsService
            .Setup(r => r.AddAsync(createDto))
            .ReturnsAsync(mount);

        // Act
        var sut = _fixture.CreateSut();
		var result = await sut.Create(createDto);
		var objectResult = result.Result as CreatedAtActionResult;
		var readDto = objectResult?.Value as MountReadDto;

		// Assert
		Assert.AreEqual(StatusCodes.Status201Created, objectResult?.StatusCode);
        Assert.AreEqual(MountType.Elephant, readDto?.Type);
	}

	[Test]
	public async Task Update_Should_ReturnNoContentResult_WhenMountExistsAndDtoIsValid()
    {
        // Arrange
        MountUpdateDto updateDto = new("Bigby Wolf", MountType.Wolf, 69);

        // Act
        var sut = _fixture.CreateSut();
		var result = await sut.Update(1, updateDto, new CancellationToken());
		var objectResult = result as NoContentResult;

		// Assert
		Assert.AreEqual(StatusCodes.Status204NoContent, objectResult?.StatusCode);
	}

	[Test]
	public async Task Update_Should_ReturnNoContentResult_WhenMountExistsAndPatchDocumentIsValid()
	{
		// Arrange
		JsonPatchDocument<MountUpdateDto> patchDocument = new();

        _fixture.MountsService
            .Setup(r => r.PatchAsync(
                1,
                It.IsAny<JsonPatchDocument<MountUpdateDto>>(),
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
	public async Task Delete_Should_ReturnNoContentResult_WhenMountExists()
    {
        // Act
        var sut = _fixture.CreateSut();
		var result = await sut.Delete(1, new CancellationToken());
		var objectResult = result as NoContentResult;

		// Assert
		Assert.AreEqual(StatusCodes.Status204NoContent, objectResult?.StatusCode);
	}
}
