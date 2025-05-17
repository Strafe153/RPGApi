using Application.Dtos;
using Application.Dtos.MountDtos;
using Domain.Shared;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
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
		_fixture.MountsService
			.GetAllAsync(Arg.Any<PageParameters>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.PageDto);

		// Act
		var result = await _fixture.MountsController.Get(_fixture.PageParameters, _fixture.CancellationToken);
		var objectResult = result.Result.As<OkObjectResult>();
		var pageDto = objectResult.Value.As<PageDto<MountReadDto>>();

		// Assert
		result.Should().NotBeNull().And.BeOfType<ActionResult<PageDto<MountReadDto>>>();
		objectResult.StatusCode.Should().Be(StatusCodes.Status200OK);
		pageDto.Entities.Count().Should().Be(_fixture.MountsCount);
	}

	[Test]
	public async Task Get_Should_ReturnActionREsultOfMountReadDto_WhenMountExists()
	{
		// Arrange
		_fixture.MountsService
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.MountReadDto);

		// Act
		var result = await _fixture.MountsController.Get(_fixture.Id, _fixture.CancellationToken);
		var objectResult = result.Result.As<OkObjectResult>();
		var readDto = objectResult.Value.As<MountReadDto>();

		// Assert
		result.Should().NotBeNull().And.BeOfType<ActionResult<MountReadDto>>();
		objectResult.StatusCode.Should().Be(StatusCodes.Status200OK);
		readDto.Should().NotBeNull();
	}

	[Test]
	public async Task Create_Should_ReturnCreatedAtActionResultOfMountReadDto_WhenDtoIsValid()
	{
		// Arrange
		_fixture.MountsService
			.AddAsync(Arg.Any<MountCreateDto>())
			.Returns(_fixture.MountReadDto);

		// Act
		var result = await _fixture.MountsController.Create(_fixture.MountCreateDto);
		var objectResult = result.Result.As<CreatedAtActionResult>();
		var readDto = objectResult.Value.As<MountReadDto>();

		// Assert
		result.Should().NotBeNull().And.BeOfType<ActionResult<MountReadDto>>();
		objectResult.StatusCode.Should().Be(StatusCodes.Status201Created);
		readDto.Should().NotBeNull();
	}

	[Test]
	public async Task Update_Should_ReturnNoContentResult_WhenMountExistsAndDtoIsValid()
	{
		// Act
		var result = await _fixture.MountsController.Update(_fixture.Id, _fixture.MountUpdateDto, _fixture.CancellationToken);
		var objectResult = result.As<NoContentResult>();

		// Assert
		result.Should().NotBeNull().And.BeOfType<NoContentResult>();
		objectResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
	}

	[Test]
	public async Task Update_Should_ReturnNoContentResult_WhenMountExistsAndPatchDocumentIsValid()
	{
		// Arrange
		_fixture.MountsService
			.PatchAsync(
				Arg.Any<int>(),
				Arg.Any<JsonPatchDocument<MountUpdateDto>>(),
				Arg.Any<Func<object, bool>>(),
				Arg.Any<CancellationToken>())
			.Returns(true);

		// Act
		var result = await _fixture.MountsController.Update(_fixture.Id, _fixture.PatchDocument, _fixture.CancellationToken);
		var objectResult = result.As<NoContentResult>();

		// Assert
		result.Should().NotBeNull().And.BeOfType<NoContentResult>();
		objectResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
	}

	[Test]
	public async Task Update_Should_ReturnObjectResult_WhenMountExistsAndPatchDocumentIsInvalid()
	{
		// Arrange
		_fixture.MountsService
			.PatchAsync(
				Arg.Any<int>(),
				Arg.Any<JsonPatchDocument<MountUpdateDto>>(),
				Arg.Any<Func<object, bool>>(),
				Arg.Any<CancellationToken>())
			.Returns(false);

		// Act
		var result = await _fixture.MountsController.Update(_fixture.Id, _fixture.PatchDocument, _fixture.CancellationToken);
		var objectResult = result.As<ObjectResult>();

		// Assert
		result.Should().NotBeNull().And.BeOfType<ObjectResult>();
		objectResult.StatusCode.Should().BeNull();
	}

	[Test]
	public async Task Delete_Should_ReturnNoContentResult_WhenMountExists()
	{
		// Act
		var result = await _fixture.MountsController.Delete(_fixture.Id, _fixture.CancellationToken);
		var objectResult = result.As<NoContentResult>();

		// Assert
		result.Should().NotBeNull().And.BeOfType<NoContentResult>();
		objectResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
	}
}
