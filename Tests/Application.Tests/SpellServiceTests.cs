using Application.Tests.Fixtures;
using Domain.Shared;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;

namespace Application.Tests;

[TestFixture]
public class SpellServiceTests
{
	private SpellServiceFixture _fixture = default!;

	[SetUp]
	public void SetUp() => _fixture = new();

	[Test]
	public async Task GetAllAsync_Should_ReturnPageDtoOfSpellReadDto_WhenDataIsValid()
	{
		// Arrange
		_fixture.SpellsRepository
			.GetAllAsync(Arg.Any<PageParameters>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.PagedList);

		// Act
		var result = await _fixture.SpellsService.GetAllAsync(_fixture.PageParameters, _fixture.CancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Entities.Should().NotBeEmpty();
	}

	[Test]
	public async Task GetByIdAsync_Should_ReturnSpellReadDto_WhenSpellExists()
	{
		// Arrange
		_fixture.SpellsRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.Spell);

		// Act
		var result = await _fixture.SpellsService.GetByIdAsync(_fixture.Id, _fixture.CancellationToken);

		// Assert
		result.Should().NotBeNull();
	}

	[Test]
	public async Task GetByIdAsync_Should_ThrowNullReferenceException_WhenSpellDoesNotExist()
	{
		// Arrange
		_fixture.SpellsRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.ReturnsNull();

		// Act
		var result = () => _fixture.SpellsService.GetByIdAsync(_fixture.Id, _fixture.CancellationToken);

		// Assert
		await result.Should().ThrowAsync<NullReferenceException>();
	}

	[Test]
	public void AddAsync_Should_ReturnSpellReadDto_WhenDtoIsValid()
	{
		// Act
		var result = _fixture.SpellsService.AddAsync(_fixture.SpellCreateDto);

		// Assert
		result.Should().NotBeNull();
	}

	[Test]
	public void UpdateAsync_Should_ReturnTask_WhenSpellExists()
	{
		// Arrange
		_fixture.SpellsRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.Spell);

		// Act
		var result = _fixture.SpellsService.UpdateAsync(_fixture.Id, _fixture.SpellUpdateDto, _fixture.CancellationToken);

		// Assert
		result.Should().NotBeNull();
	}

	[Test]
	public async Task UpdateAsync_Should_ThrowNullReferenceException_WhenSpellDoesNotExist()
	{
		// Arrange
		_fixture.SpellsRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.ReturnsNull();

		// Act
		var result = () => _fixture.SpellsService.UpdateAsync(
			_fixture.Id,
			_fixture.SpellUpdateDto,
			_fixture.CancellationToken);

		// Assert
		await result.Should().ThrowAsync<NullReferenceException>();
	}

	[Test]
	public void DeleteAsync_Should_ReturnTask_WhenSpellExists()
	{
		// Arrange
		_fixture.SpellsRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.Spell);

		// Act
		var result = _fixture.SpellsService.DeleteAsync(_fixture.Id, _fixture.CancellationToken);

		// Assert
		result.Should().NotBeNull();
	}

	[Test]
	public async Task DeleteAsync_Should_ThrowNullReferenceException_WhenSpellDoesNotExist()
	{
		// Arrange
		_fixture.SpellsRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.ReturnsNull();

		// Act
		var result = () => _fixture.SpellsService.DeleteAsync(_fixture.Id, _fixture.CancellationToken);

		// Assert
		await result.Should().ThrowAsync<NullReferenceException>();
	}

	[Test]
	public async Task PatchAsync_Should_ReturnTrue_WhenSpellExistsAndModelIsValid()
	{
		// Arrange
		_fixture.SpellsRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.Spell);

		// Act
		var result = await _fixture.SpellsService.PatchAsync(
			_fixture.Id,
			_fixture.PatchDocument,
			_ => true,
			_fixture.CancellationToken);

		// Assert
		result.Should().BeTrue();
	}

	[Test]
	public async Task PatchAsync_Should_ReturnFalse_WhenSpellExistsAndModelIsNotValid()
	{
		// Arrange
		_fixture.SpellsRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.Spell);

		// Act
		var result = await _fixture.SpellsService.PatchAsync(
			_fixture.Id,
			_fixture.PatchDocument,
			_ => false,
			_fixture.CancellationToken);

		// Assert
		result.Should().BeFalse();
	}

	[Test]
	public async Task PatchAsync_Should_ThrowNullReferenceException_WhenSpellDoesNotExist()
	{
		// Arrange
		_fixture.SpellsRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.ReturnsNull();

		// Act
		var result = () => _fixture.SpellsService.PatchAsync(
			_fixture.Id,
			_fixture.PatchDocument,
			_ => false,
			_fixture.CancellationToken);

		// Assert
		await result.Should().ThrowAsync<NullReferenceException>();
	}
}
