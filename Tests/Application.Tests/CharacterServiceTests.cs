using Application.Tests.Fixtures;
using Domain.Dtos;
using Domain.Dtos.CharacterDtos;
using Domain.Enums;
using Domain.Shared;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;

namespace Application.Tests;

[TestFixture]
public class CharacterServiceTests
{
	private CharacterServiceFixture _fixture = default!;
	private static int _characterId;

	[SetUp]
	public void SetUp()
	{
		 _fixture = new();
		_characterId = _fixture.CharacterId;
	}

	[TearDown]
	public void TearDown()
	{
		_fixture.Character.Health = 100;
	}

	[Test]
	public async Task GetAllAsync_Should_ReturnPageDtoOfCharacterReadDto_WhenDataIsValid()
	{
		// Arrange
		_fixture.CharactersRepository
			.GetAllAsync(Arg.Any<PageParameters>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.PagedList);

		// Act
		var result = await _fixture.CharactersService.GetAllAsync(_fixture.PageParameters, _fixture.CancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Entities.Should().NotBeEmpty();
	}

	[Test]
	public async Task GetByIdAsync_Should_ReturnCharacterReadDto_WhenCharacterExists()
	{
		// Arrange
		_fixture.CharactersRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.Character);

		// Act
		var result = await _fixture.CharactersService.GetByIdAsync(_fixture.CharacterId, _fixture.CancellationToken);

		// Assert
		result.Should().NotBeNull();
	}

	[Test]
	public async Task GetByIdAsync_Should_ThrowNullReferenceException_WhenCharacterDoesNotExist()
	{
		// Arrange
		_fixture.CharactersRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.ReturnsNull();

		// Act
		var result = () => _fixture.CharactersService.GetByIdAsync(_fixture.CharacterId, _fixture.CancellationToken);

		// Assert
		await result.Should().ThrowAsync<NullReferenceException>();
	}

	[Test]
	public void AddAsync_Should_ReturnCharacterReadDto_WhenPlayerExists()
	{
		// Arrange
		_fixture.CharactersRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.Character);

		// Act
		var result = _fixture.CharactersService.AddAsync(_fixture.CharacterCreateDto, _fixture.CancellationToken);

		// Assert
		result.Should().NotBeNull();
	}

	[Test]
	public async Task AddAsync_Should_ThrowNullReferenceException_WhenPlayerDoesNotExist()
	{
		// Arrange
		_fixture.PlayersRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.ReturnsNull();

		// Act
		var result = () => _fixture.CharactersService.AddAsync(_fixture.CharacterCreateDto, _fixture.CancellationToken);

		// Assert
		await result.Should().ThrowAsync<NullReferenceException>();
	}

	[Test]
	public void UpdateAsync_Should_ReturnTask_WhenCharacterExists()
	{
		// Arrange
		_fixture.CharactersRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.Character);

		// Act
		var result = _fixture.CharactersService.UpdateAsync(
			_fixture.CharacterId,
			_fixture.CharacterUpdateDto,
			_fixture.CancellationToken);

		// Assert
		result.Should().NotBeNull();
	}

	[Test]
	public async Task UpdateAsync_Should_ThrowNullReferenceExceition_WhenCharacterDoesNotExist()
	{
		// Arrange
		_fixture.CharactersRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.ReturnsNull();

		// Act
		var result = () => _fixture.CharactersService.UpdateAsync(
			_fixture.CharacterId,
			_fixture.CharacterUpdateDto,
			_fixture.CancellationToken);

		// Assert
		await result.Should().ThrowAsync<NullReferenceException>();
	}

	[Test]
	public void DeleteAsync_Should_ReturnTask_WhenCharacterExists()
	{
		// Arrange
		_fixture.CharactersRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.Character);

		// Act
		var result = _fixture.CharactersService.DeleteAsync(_fixture.CharacterId, _fixture.CancellationToken);

		// Assert
		result.Should().NotBeNull();
	}

	[Test]
	public async Task DeleteAsync_Should_ThrowNullReferenceException_WhenCharacterDoesNotExist()
	{
		// Arrange
		_fixture.CharactersRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.ReturnsNull();

		// Act
		var result = () => _fixture.CharactersService.DeleteAsync(_fixture.CharacterId, _fixture.CancellationToken);

		// Assert
		await result.Should().ThrowAsync<NullReferenceException>();
	}

	[Test]
	public async Task PatchAsync_Should_ReturnTrue_WhenCharacterExistsAndModelIsValid()
	{
		// Arrange
		_fixture.CharactersRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.Character);

		// Act
		var result = await _fixture.CharactersService.PatchAsync(
			_fixture.CharacterId,
			_fixture.PatchDocument,
			_ => true,
			_fixture.CancellationToken);

		// Assert
		result.Should().BeTrue();
	}

	[Test]
	public async Task PatchAsync_Should_ReturnFalse_WhenCharacterExistsAndModelIsNotValid()
	{
		// Arrange
		_fixture.CharactersRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.Character);

		// Act
		var result = await _fixture.CharactersService.PatchAsync(
			_fixture.CharacterId,
			_fixture.PatchDocument,
			_ => false,
			_fixture.CancellationToken);

		// Assert
		result.Should().BeFalse();
	}

	[Test]
	public async Task PatchAsync_Should_ThrowNullReferenceException_WhenCharacterDoesNotExist()
	{
		// Arrange
		_fixture.CharactersRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.ReturnsNull();

		// Act
		var result = () => _fixture.CharactersService.PatchAsync(
			_fixture.CharacterId,
			_fixture.PatchDocument,
			_ => false,
			_fixture.CancellationToken);

		// Assert
		await result.Should().ThrowAsync<NullReferenceException>();
	}

	[Test]
	[TestCaseSource(nameof(ManageItemAsyncTestData))]
	public void ManageItemAsync_Should_ReturnTask_WhenCharacterAndItemExist(ManageItemDto itemDto)
	{
		// Arrange
		_fixture.CharactersRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.Character);

		_fixture.WeaponsRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.Weapon);

		_fixture.SpellsRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.Spell);

		_fixture.MountsRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.Mount);

		// Act
		var result = _fixture.CharactersService.ManageItemAsync(itemDto, _fixture.CancellationToken);

		// Assert
		result.Should().NotBeNull();

	}

	[Test]
	[TestCaseSource(nameof(ManageItemAsyncTestData))]
	public void ManageItemAsync_Should_ReturnTask_WhenCharacterExistAndItemDoesNotExist(ManageItemDto itemDto)
	{
		// Arrange
		_fixture.CharactersRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.Character);

		_fixture.WeaponsRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.ReturnsNull();

		_fixture.SpellsRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.ReturnsNull();

		_fixture.MountsRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.ReturnsNull();

		// Act
		var result = _fixture.CharactersService.ManageItemAsync(itemDto, _fixture.CancellationToken);

		// Assert
		result.Should().NotBeNull();

	}

	[Test]
	[TestCaseSource(nameof(ManageItemAsyncTestData))]
	public async Task ManageItemAsync_Should_ThrowNullReferenceException_WhenCharacterDoesNotExist(ManageItemDto itemDto)
	{
		// Arrange
		_fixture.CharactersRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.ReturnsNull();

		// Act
		var result = () => _fixture.CharactersService.ManageItemAsync(itemDto, _fixture.CancellationToken);

		// Assert
		await result.Should().ThrowAsync<NullReferenceException>();
	}

	[Test]
	[TestCaseSource(nameof(HitAsyncSuccessfulTestData))]
	public void HitAsync_Should_ReturnTask_WhenCharacterAndItemExist(
		HitDto hitDto,
		int damage,
		int startingHealth,
		int resultingHealth)
	{
		// Arrange
		_fixture.CharactersRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.Character);

		_fixture.WeaponsRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.Weapon);

		_fixture.SpellsRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.Spell);

		_fixture.Weapon.Damage = _fixture.Spell.Damage = damage;
		_fixture.Character.Health = startingHealth;

		// Act
		var result = _fixture.CharactersService.HitAsync(hitDto, _fixture.CancellationToken);

		// Assert
		result.Should().NotBeNull();
		_fixture.Character.Health.Should().Be(resultingHealth);
	}

	[Test]
	[TestCaseSource(nameof(HitAsyncFailedTestData))]
	public async Task HitAsync_Should_ThrowNullReferenceException_WhenCharacterDoesNotExist(HitDto hitDto)
	{
		// Arrange
		_fixture.CharactersRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.ReturnsNull();

		// Act
		var result = () => _fixture.CharactersService.HitAsync(hitDto, _fixture.CancellationToken);

		// Assert
		await result.Should().ThrowAsync<NullReferenceException>();
	}

	[Test]
	[TestCaseSource(nameof(HitAsyncFailedTestData))]
	public async Task HitAsync_Should_ThrowNullReferenceException_WhenCharacterExistsAndItemDoesNotExist(HitDto hitDto)
	{
		// Arrange
		_fixture.CharactersRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.Character);

		_fixture.WeaponsRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.ReturnsNull();

		_fixture.SpellsRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.ReturnsNull();

		// Act
		var result = () => _fixture.CharactersService.HitAsync(hitDto, _fixture.CancellationToken);

		// Assert
		await result.Should().ThrowAsync<NullReferenceException>();
	}

	private static IEnumerable<ManageItemDto> ManageItemAsyncTestData()
	{
		yield return new(_characterId, 1, ItemType.Weapon, ManageItemOperation.Add);
		yield return new(_characterId, 1, ItemType.Weapon, ManageItemOperation.Remove);
		yield return new(_characterId, 1, ItemType.Spell, ManageItemOperation.Add);
		yield return new(_characterId, 1, ItemType.Spell, ManageItemOperation.Remove);
		yield return new(_characterId, 1, ItemType.Mount, ManageItemOperation.Add);
		yield return new(_characterId, 1, ItemType.Mount, ManageItemOperation.Remove);
	}

	private static IEnumerable<HitDto> HitAsyncFailedTestData()
	{
		yield return new(1, _characterId, 1, HitType.Weapon);
		yield return new(1, _characterId, 1, HitType.Weapon);
		yield return new(1, _characterId, 1, HitType.Spell);
		yield return new(1, _characterId, 1, HitType.Spell);
	}

	private static IEnumerable<object[]> HitAsyncSuccessfulTestData()
	{
		yield return new object[]
		{
			new HitDto(1, _characterId, 1, HitType.Weapon),
			38,
			100,
			62
		};

		yield return new object[]
		{
			new HitDto(1, _characterId, 1, HitType.Weapon),
			94,
			75,
			0
		};

		yield return new object[]
		{
			new HitDto(1, _characterId, 1, HitType.Spell),
			47,
			100,
			53
		};

		yield return new object[]
		{
			new HitDto(1, _characterId, 1, HitType.Spell),
			28,
			16,
			0
		};
	}
}
