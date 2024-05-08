using Application.Tests.Fixtures;
using Domain.Entities;
using Domain.Shared;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;

namespace Application.Tests;

[TestFixture]
public class MountServiceTests
{
	private MountServiceFixture _fixture = default!;

	[SetUp]
	public void SetUp() => _fixture = new();

	[Test]
	public async Task GetAllAsync_Should_ReturnPageDtoOfMountReadDto_WhenDataIsValid()
	{
		// Arrange
		_fixture.MountsRepository
			.GetAllAsync(Arg.Any<PageParameters>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.PagedList);

		// Act
		var result = await _fixture.MountsService.GetAllAsync(_fixture.PageParameters, _fixture.CancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Entities.Should().NotBeEmpty();
	}

	[Test]
	public async Task GetByIdAsync_Should_ReturnMountReadDto_WhenMountsExists()
	{
		// Arrange
		_fixture.MountsRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.Mount);

		// Act
		var result = await _fixture.MountsService.GetByIdAsync(_fixture.Id, _fixture.CancellationToken);

		// Assert
		result.Should().NotBeNull();
	}

	[Test]
	public async Task GetByIdAsync_Should_ThrowNullReferenceException_WhenMountDoesNotExist()
	{
		// Arrange
		_fixture.MountsRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.ReturnsNull();

		// Act
		var result = () => _fixture.MountsService.GetByIdAsync(_fixture.Id, _fixture.CancellationToken);

		// Assert
		await result.Should().ThrowAsync<NullReferenceException>();
	}

	[Test]
	public void AddAsync_Should_ReturnMountReadDto_WhenDtoIsValid()
	{
		// Act
		var result = _fixture.MountsService.AddAsync(_fixture.MountCreateDto);

		// Assert
		result.Should().NotBeNull();
	}

	[Test]
	public void UpdateAsync_Should_ReturnTask_WhenMountExists()
	{
		// Arrange
		_fixture.MountsRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.Mount);

		// Act
		var result = _fixture.MountsService.UpdateAsync(_fixture.Id, _fixture.MountUpdateDto, _fixture.CancellationToken);

		// Assert
		result.Should().NotBeNull();
	}

	[Test]
	public async Task UpdateAsync_Should_ThrowNullReferenceExcpetion_WhenMountDoesNotExist()
	{
		// Arrange
		_fixture.MountsRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.ReturnsNull();

		// Act
		var result = () => _fixture.MountsService.UpdateAsync(_fixture.Id, _fixture.MountUpdateDto, _fixture.CancellationToken);

		// Assert
		await result.Should().ThrowAsync<NullReferenceException>();
	}

	[Test]
	public void DeleteAsync_Should_ReturnTask_WhenMountExists()
	{
		// Arrange
		_fixture.MountsRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.Mount);

		// Act
		var result = _fixture.MountsService.DeleteAsync(_fixture.Id, _fixture.CancellationToken);

		// Assert
		result.Should().NotBeNull();
	}

	[Test]
	public async Task DeleteAsync_Should_ThrowNullReferenceException_WhenMountDoesNotExist()
	{
		// Arrange
		_fixture.MountsRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.ReturnsNull();

		// Act
		var result = () => _fixture.MountsService.DeleteAsync(_fixture.Id, _fixture.CancellationToken);

		// Assert
		await result.Should().ThrowAsync<NullReferenceException>();
	}

	[Test]
	public async Task PatchAsync_Should_ReturnTrue_WhenMountExistsAndModelIsValid()
	{
		// Arrange
		_fixture.MountsRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.Mount);

		// Act
		var result = await _fixture.MountsService.PatchAsync(
			_fixture.Id,
			_fixture.PatchDocument,
			_ => true,
			_fixture.CancellationToken);

		// Assert
		result.Should().BeTrue();
	}

	[Test]
	public async Task PatchAsync_Should_ReturnFalse_WhenMountExistsAndModelIsNotValid()
	{
		// Arrange
		_fixture.MountsRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.Returns(_fixture.Mount);

		// Act
		var result = await _fixture.MountsService.PatchAsync(
			_fixture.Id,
			_fixture.PatchDocument,
			_ => false,
			_fixture.CancellationToken);

		// Assert
		result.Should().BeFalse();
	}

	[Test]
	public async Task PatchAsync_Should_ThrowNullReferenceException_WhenMountDoesNotExist()
	{
		// Arrange
		_fixture.MountsRepository
			.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
			.ReturnsNull();

		// Act
		var result = () => _fixture.MountsService.PatchAsync(
			_fixture.Id,
			_fixture.PatchDocument,
			_ => false,
			_fixture.CancellationToken);

		// Assert
		await result.Should().ThrowAsync<NullReferenceException>();
	}
}
