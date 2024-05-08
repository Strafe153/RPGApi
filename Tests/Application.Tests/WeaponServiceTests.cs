using Application.Tests.Fixtures;
using Domain.Shared;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;

namespace Application.Tests;

[TestFixture]
public class WeaponServiceTests
{
	private WeaponServiceFixture _fixture = default!;

	[SetUp]
	public void SetUp() => _fixture = new();

	[Test]
	public async Task GetAllAsync_Should_ReturnPageDtoOfWeaponReadDto_WhenDataIsValid()
	{
		// Arrange
		_fixture.WeaponsRepository
			.GetAllAsync(Arg.Any<PageParameters>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.PagedList);

		// Act
		var result = await _fixture.WeaponsService.GetAllAsync(_fixture.PageParameters, _fixture.CancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Entities.Should().NotBeEmpty();
	}

	[Test]
	public async Task GetByIdAsync_Should_ReturnWeaponReadDto_WhenWeaponExists()
	{
		// Arrange
		_fixture.WeaponsRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.Weapon);

		// Act
		var result = await _fixture.WeaponsService.GetByIdAsync(_fixture.Id, _fixture.CancellationToken);

		// Assert
		result.Should().NotBeNull();
	}

	[Test]
	public async Task GetByIdAsync_Should_ThrowNullReferenceException_WhenWeaponDoesNotExist()
	{
		// Arrange
		_fixture.WeaponsRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.ReturnsNull();

		// Act
		var result = () => _fixture.WeaponsService.GetByIdAsync(_fixture.Id, _fixture.CancellationToken);

		// Assert
		await result.Should().ThrowAsync<NullReferenceException>();
	}

	[Test]
	public void AddAsync_Should_ReturnWeaponReadDto_WhenDtoIsValid()
	{
		// Act
		var result = _fixture.WeaponsService.AddAsync(_fixture.WeaponCreateDto);

		// Assert
		result.Should().NotBeNull();
	}

	[Test]
	public void UpdateAsync_Should_ReturnTask_WhenWeaponExists()
	{
		// Arrange
		_fixture.WeaponsRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.Weapon);

		// Act
		var result = _fixture.WeaponsService.UpdateAsync(_fixture.Id, _fixture.WeaponUpdateDto, _fixture.CancellationToken);

		// Assert
		result.Should().NotBeNull();
	}

	[Test]
	public async Task UpdateAsync_Should_ThrowNullReferenceException_WhenWeaponDoesNotExist()
	{
		// Arrange
		_fixture.WeaponsRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.ReturnsNull();

		// Act
		var result = () => _fixture.WeaponsService.UpdateAsync(
			_fixture.Id,
			_fixture.WeaponUpdateDto,
			_fixture.CancellationToken);

		// Assert
		await result.Should().ThrowAsync<NullReferenceException>();
	}

	[Test]
	public void DeleteAsync_Should_ReturnTask_WhenWeaponExists()
	{
		// Arrange
		_fixture.WeaponsRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.Weapon);

		// Act
		var result = _fixture.WeaponsService.DeleteAsync(_fixture.Id, _fixture.CancellationToken);

		// Assert
		result.Should().NotBeNull();
	}

	[Test]
	public async Task DeleteAsync_Should_ThrowNullReferenceException_WhenWeaponDoesNotExist()
	{
		// Arrange
		_fixture.WeaponsRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.ReturnsNull();

		// Act
		var result = () => _fixture.WeaponsService.DeleteAsync(_fixture.Id, _fixture.CancellationToken);

		// Assert
		await result.Should().ThrowAsync<NullReferenceException>();
	}

	[Test]
	public async Task PatchAsync_Should_ReturnTrue_WhenWeaponExistsAndModelIsValid()
	{
		// Arrange
		_fixture.WeaponsRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.Weapon);

		// Act
		var result = await _fixture.WeaponsService.PatchAsync(
			_fixture.Id,
			_fixture.PatchDocument,
			_ => true,
			_fixture.CancellationToken);

		// Assert
		result.Should().BeTrue();
	}

	[Test]
	public async Task PatchAsync_Should_ReturnFalse_WhenWeaponExistsAndModelIsNotValid()
	{
		// Arrange
		_fixture.WeaponsRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.Weapon);

		// Act
		var result = await _fixture.WeaponsService.PatchAsync(
			_fixture.Id,
			_fixture.PatchDocument,
			_ => false,
			_fixture.CancellationToken);

		// Assert
		result.Should().BeFalse();
	}

	[Test]
	public async Task PatchAsync_Should_ThrowNullReferenceException_WhenWeaponDoesNotExist()
	{
		// Arrange
		_fixture.WeaponsRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.ReturnsNull();

		// Act
		var result = () => _fixture.WeaponsService.PatchAsync(
			_fixture.Id,
			_fixture.PatchDocument,
			_ => false,
			_fixture.CancellationToken);

		// Assert
		await result.Should().ThrowAsync<NullReferenceException>();
	}
}
