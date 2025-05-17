using Application.Dtos;
using Application.Dtos.WeaponDtos;
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
public class WeaponControllerTests
{
	private WeaponsControllerFixture _fixture = default!;

	[SetUp]
	public void SetUp() => _fixture = new();

	[Test]
	public async Task Get_Should_ReturnActionResultOfPageDtoOfWeaponReadDto_WhenDataIsValid()
	{
		// Arrange
		_fixture.WeaponsService
			.GetAllAsync(Arg.Any<PageParameters>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.PageDto);

		// Act
		var result = await _fixture.WeaponsController.Get(_fixture.PageParameters, _fixture.CancellationToken);
		var objectResult = result.Result.As<OkObjectResult>();
		var pageDto = objectResult.Value.As<PageDto<WeaponReadDto>>();

		// Assert
		result.Should().NotBeNull().And.BeOfType<ActionResult<PageDto<WeaponReadDto>>>();
		objectResult.StatusCode.Should().Be(StatusCodes.Status200OK);
		pageDto.Entities.Count().Should().Be(_fixture.WeaponsCount);
	}

	[Test]
	public async Task Get_Should_ReturnActionResultOfWeaponReadDto_WhenWeaponExists()
	{
		// Arrange
		_fixture.WeaponsService
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.WeaponReadDto);

		// Act
		var result = await _fixture.WeaponsController.Get(_fixture.Id, _fixture.CancellationToken);
		var objectResult = result.Result.As<OkObjectResult>();
		var readDto = objectResult.Value.As<WeaponReadDto>();

		// Assert
		result.Should().NotBeNull().And.BeOfType<ActionResult<WeaponReadDto>>();
		objectResult.StatusCode.Should().Be(StatusCodes.Status200OK);
		readDto.Should().NotBeNull();
	}

	[Test]
	public async Task Create_Should_ReturnCreatedAtActionResultOfWeaponReadDto_WhenDataIsValid()
	{
		// Arrange
		_fixture.WeaponsService
			.AddAsync(Arg.Any<WeaponCreateDto>())
			.Returns(_fixture.WeaponReadDto);

		// Act
		var result = await _fixture.WeaponsController.Create(_fixture.WeaponCreateDto);
		var objectResult = result.Result.As<CreatedAtActionResult>();
		var readDto = objectResult.Value.As<WeaponReadDto>();

		// Assert
		result.Should().NotBeNull().And.BeOfType<ActionResult<WeaponReadDto>>();
		objectResult.StatusCode.Should().Be(StatusCodes.Status201Created);
		readDto.Should().NotBeNull();
	}

	[Test]
	public async Task Update_Should_ReturnNoContentResult_WhenWeaponExistsAndDtoIsValid()
	{
		// Act
		var result = await _fixture.WeaponsController.Update(_fixture.Id, _fixture.WeaponUpdateDto, _fixture.CancellationToken);
		var objectResult = result.As<NoContentResult>();

		// Assert
		result.Should().NotBeNull().And.BeOfType<NoContentResult>();
		objectResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
	}

	[Test]
	public async Task Update_Should_ReturnNoContentResult_WhenWeaponExistsAndPatchDocumentIsValid()
	{
		// Arrange
		_fixture.WeaponsService
			.PatchAsync(
				Arg.Any<int>(),
				Arg.Any<JsonPatchDocument<WeaponUpdateDto>>(),
				Arg.Any<Func<object, bool>>(),
				Arg.Any<CancellationToken>())
			.Returns(true);

		// Act
		var result = await _fixture.WeaponsController.Update(_fixture.Id, _fixture.PatchDocument, _fixture.CancellationToken);
		var objectResult = result.As<NoContentResult>();

		// Assert
		result.Should().NotBeNull().And.BeOfType<NoContentResult>();
		objectResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
	}

	[Test]
	public async Task Update_Should_ReturnObjectResult_WhenWeaponExistsAndPatchDocumentIsInvalid()
	{
		// Arrange
		_fixture.WeaponsService
			.PatchAsync(
				Arg.Any<int>(),
				Arg.Any<JsonPatchDocument<WeaponUpdateDto>>(),
				Arg.Any<Func<object, bool>>(),
				Arg.Any<CancellationToken>())
			.Returns(false);

		// Act
		var result = await _fixture.WeaponsController.Update(_fixture.Id, _fixture.PatchDocument, _fixture.CancellationToken);
		var objectResult = result.As<ObjectResult>();

		// Assert
		result.Should().NotBeNull().And.BeOfType<ObjectResult>();
		objectResult.StatusCode.Should().BeNull();
	}

	[Test]
	public async Task Delete_Should_ReturnNoContentResult_WhenWeaponExists()
	{
		// Act
		var result = await _fixture.WeaponsController.Delete(_fixture.Id, _fixture.CancellationToken);
		var objectResult = result.As<NoContentResult>();

		// Assert
		result.Should().NotBeNull().And.BeOfType<NoContentResult>();
		objectResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
	}
}
