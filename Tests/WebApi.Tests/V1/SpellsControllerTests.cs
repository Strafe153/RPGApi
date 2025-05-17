using Application.Dtos;
using Application.Dtos.SpellDtos;
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
public class SpellsControllerTests
{
	private SpellsControllerFixture _fixture = default!;

	[SetUp]
	public void SetUp() => _fixture = new();

	[Test]
	public async Task Get_Should_ReturnActionResultOfPageDtoOfSpellReadDto_WhenDataIsValid()
	{
		// Arrange
		_fixture.SpellsService
			.GetAllAsync(Arg.Any<PageParameters>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.PageDto);

		// Act
		var result = await _fixture.SpellsController.Get(_fixture.PageParameters, _fixture.CancellationToken);
		var objectResult = result.Result.As<OkObjectResult>();
		var pageDto = objectResult.Value.As<PageDto<SpellReadDto>>();

		// Assert
		result.Should().NotBeNull().And.BeOfType<ActionResult<PageDto<SpellReadDto>>>();
		objectResult.StatusCode.Should().Be(StatusCodes.Status200OK);
		pageDto.Entities.Count().Should().Be(_fixture.SpellsCount);
	}

	[Test]
	public async Task Get_Should_ReturnActionResultOfSpellReadDto_WhenSpellExists()
	{
		// Arrange
		_fixture.SpellsService
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.SpellReadDto);

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
	public async Task Create_Should_ReturnCreatedAtActionResultOfSpellReadDto_WhenDtoIsValid()
	{
		// Arrange
		_fixture.SpellsService
			.AddAsync(Arg.Any<SpellCreateDto>())
			.Returns(_fixture.SpellReadDto);

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
	public async Task Update_Should_ReturnNoContentResult_WhenSpellExistsAndDtoIsValid()
	{
		// Act
		var result = await _fixture.SpellsController.Update(_fixture.Id, _fixture.SpellUpdateDto, _fixture.CancellationToken);
		var objectResult = result.As<NoContentResult>();

		// Assert
		result.Should().NotBeNull().And.BeOfType<NoContentResult>();
		objectResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
	}

	[Test]
	public async Task Update_Should_ReturnNoContentResult_WhenSpellExistsAndPatchDocumentIsValid()
	{
		// Arrange
		_fixture.SpellsService
			.PatchAsync(
				Arg.Any<int>(),
				Arg.Any<JsonPatchDocument<SpellUpdateDto>>(),
				Arg.Any<Func<object, bool>>(),
				Arg.Any<CancellationToken>())
			.Returns(true);

		// Act
		var result = await _fixture.SpellsController.Update(_fixture.Id, _fixture.PatchDocument, _fixture.CancellationToken);
		var objectResult = result.As<NoContentResult>();

		// Assert
		result.Should().NotBeNull().And.BeOfType<NoContentResult>();
		objectResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
	}

	[Test]
	public async Task Update_Should_ReturnObjectResult_WhenSpellExistsAndPatchDocumentIsInvalid()
	{
		// Arrange
		_fixture.SpellsService
			.PatchAsync(
				Arg.Any<int>(),
				Arg.Any<JsonPatchDocument<SpellUpdateDto>>(),
				Arg.Any<Func<object, bool>>(),
				Arg.Any<CancellationToken>())
			.Returns(false);

		// Act
		var result = await _fixture.SpellsController.Update(_fixture.Id, _fixture.PatchDocument, _fixture.CancellationToken);
		var objectResult = result.As<ObjectResult>();

		// Assert
		result.Should().NotBeNull().And.BeOfType<ObjectResult>();
		objectResult.StatusCode.Should().BeNull();
	}

	[Test]
	public async Task Delete_Should_ReturnNoContentResult_WhenSpellExists()
	{
		// Act
		var result = await _fixture.SpellsController.Delete(_fixture.Id, _fixture.CancellationToken);
		var objectResult = result.As<NoContentResult>();

		// Assert
		result.Should().NotBeNull().And.BeOfType<NoContentResult>();
		objectResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
	}
}
