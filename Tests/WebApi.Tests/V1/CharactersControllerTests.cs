using Domain.Dtos;
using Domain.Dtos.CharacterDtos;
using Domain.Enums;
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
public class CharactersControllerTests
{
	private CharactersControllerFixture _fixture = default!;
	private static int _characterId;

	[SetUp]
	public void SetUp()
	{
		_fixture = new();
		_characterId = _fixture.Id;
	}

	[Test]
	public async Task Get_Should_ReturnActionResultOfPageDtoOfCharacterReadDto_WhenDataIsValid()
	{
		// Arrange
		_fixture.CharactersService
			.GetAllAsync(Arg.Any<PageParameters>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.PageDto);

		// Act
		var result = await _fixture.CharactersController.Get(_fixture.PageParameters, _fixture.CancellationToken);
		var objectResult = result.Result.As<OkObjectResult>();
		var pageDto = objectResult.Value.As<PageDto<CharacterReadDto>>();

		result.Should().NotBeNull().And.BeOfType<ActionResult<PageDto<CharacterReadDto>>>();
		objectResult.StatusCode.Should().Be(StatusCodes.Status200OK);
		pageDto.Entities.Count().Should().Be(_fixture.CharactersCount);
	}

	[Test]
	public async Task Get_Should_ReturnCreatedAtActionResultOfCharacterReadDto_WhenCharacterExists()
	{
		// Arrange
		_fixture.CharactersService
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.CharacterReadDto);

		// Act
		var result = await _fixture.CharactersController.Get(_fixture.Id, _fixture.CancellationToken);
		var objectResult = result.Result.As<OkObjectResult>();
		var readDto = objectResult.Value.As<CharacterReadDto>();

		// Assert
		result.Should().NotBeNull().And.BeOfType<ActionResult<CharacterReadDto>>();
		objectResult.StatusCode.Should().Be(StatusCodes.Status200OK);
		readDto.Should().NotBeNull();
	}

	[Test]
	public async Task Create_Should_ReturnActionResultOfCharacterReadDto_WhenCharacterCreateDtoIsValid()
	{
		// Arrange
		_fixture.CharactersService
			.AddAsync(Arg.Any<CharacterCreateDto>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.CharacterReadDto);

		// Act
		var result = await _fixture.CharactersController.Create(_fixture.CharacterCreateDto, _fixture.CancellationToken);
		var objectResult = result.Result.As<CreatedAtActionResult>();
		var readDto = objectResult.Value.As<CharacterReadDto>();

		// Assert
		result.Should().NotBeNull().And.BeOfType<ActionResult<CharacterReadDto>>();
		objectResult.StatusCode.Should().Be(StatusCodes.Status201Created);
		readDto.Should().NotBeNull();
	}

	[Test]
	public async Task Update_Should_ReturnNoContentResult_WhenCharacterExistsAndDtoIsValid()
	{
		// Arrange
		_fixture.CharactersService
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.CharacterReadDto);

		// Act
		var result = await _fixture.CharactersController.Update(_fixture.Id, _fixture.CharacterUpdateDto, _fixture.CancellationToken);
		var objectResult = result.As<NoContentResult>();

		// Assert
		result.Should().NotBeNull().And.BeOfType<NoContentResult>();
		objectResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
	}

	[Test]
	public async Task Update_Should_ReturnNoContentResult_WhenCharacterExistsAndPatchDocumentIsValid()
	{
		// Arrange
		_fixture.CharactersService
			.PatchAsync(
				Arg.Any<int>(),
				Arg.Any<JsonPatchDocument<CharacterUpdateDto>>(),
				Arg.Any<Func<object, bool>>(),
				Arg.Any<CancellationToken>())
			.Returns(true);

		// Act
		var result = await _fixture.CharactersController.Update(_fixture.Id, _fixture.PatchDocument, _fixture.CancellationToken);
		var objectResult = result.As<NoContentResult>();

		// Assert
		result.Should().NotBeNull().And.BeOfType<NoContentResult>();
		objectResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
	}

	[Test]
	public async Task Update_Should_ReturnObjectResult_WhenCharacterExistsAndPatchDocumentIsInvalid()
	{
		// Arrange
		_fixture.CharactersService
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.CharacterReadDto);

		// Act
		var result = await _fixture.CharactersController.Update(_fixture.Id, _fixture.PatchDocument, _fixture.CancellationToken);
		var objectResult = result.As<ObjectResult>();

		// Assert
		result.Should().NotBeNull().And.BeOfType<ObjectResult>();
		objectResult.StatusCode.Should().BeNull();
	}

	[Test]
	public async Task Delete_Should_ReturnNoContentResult_WhenCharacterExists()
	{
		// Act
		var result = await _fixture.CharactersController.Delete(_fixture.Id, _fixture.CancellationToken);
		var objectResult = result.As<NoContentResult>();

		// Assert
		result.Should().NotBeNull().And.BeOfType<NoContentResult>();
		objectResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
	}

	[Test]
	[TestCaseSource(nameof(ManageItemTestData))]
	public async Task ManageItem_Should_ReturnNoContentResult_WhenCharacterAndItemExist(ManageItemDto itemDto)
	{
		// Act
		var result = await _fixture.CharactersController.ManageItem(itemDto, _fixture.CancellationToken);
		var objectResult = result.As<NoContentResult>();

		// Assert
		result.Should().NotBeNull().And.BeOfType<NoContentResult>();
		objectResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
	}

	[Test]
	[TestCaseSource(nameof(HitTestData))]
	public async Task Hit_Should_ReturnNoContentResult_WhenCharacterAndItemExist(HitDto hitDto)
	{
		// Act
		var result = await _fixture.CharactersController.Hit(hitDto, _fixture.CancellationToken);
		var objectResult = result.As<NoContentResult>();

		// Assert
		result.Should().NotBeNull().And.BeOfType<NoContentResult>();
		objectResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
	}

	private static IEnumerable<ManageItemDto> ManageItemTestData()
	{
		yield return new(_characterId, 1, ItemType.Weapon, ManageItemOperation.Add);
		yield return new(_characterId, 1, ItemType.Weapon, ManageItemOperation.Remove);
		yield return new(_characterId, 1, ItemType.Spell, ManageItemOperation.Add);
		yield return new(_characterId, 1, ItemType.Spell, ManageItemOperation.Remove);
		yield return new(_characterId, 1, ItemType.Mount, ManageItemOperation.Add);
		yield return new(_characterId, 1, ItemType.Mount, ManageItemOperation.Remove);
	}

	private static IEnumerable<HitDto> HitTestData()
	{
		yield return new(1, _characterId, 1, HitType.Weapon);
		yield return new(1, _characterId, 1, HitType.Spell);
	}
}
