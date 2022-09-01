using Application.Tests.Fixtures;
using Core.Entities;
using Core.Exceptions;
using Core.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.Tests
{
    public class MountServiceTests : IClassFixture<MountServiceFixture>
    {
        private readonly MountServiceFixture _fixture;

        public MountServiceTests(MountServiceFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetAllAsync_ValidParameters_ReturnsPaginatedListOfMount()
        {
            // Arrange
            _fixture.MockMountRepository
                .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(_fixture.PaginatedList);

            // Act
            var result = await _fixture.MockMountService.GetAllAsync(_fixture.Id, _fixture.Id);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Should().BeOfType<PaginatedList<Mount>>();
        }

        [Fact]
        public async Task GetByIdAsync_ExistingMount_ReturnsMount()
        {
            // Arrange
            _fixture.MockMountRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Mount);

            // Act
            var result = await _fixture.MockMountService.GetByIdAsync(_fixture.Id);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Mount>();
        }

        [Fact]
        public async Task GetByIdAsync_NonexistingMount_ThrowsNullReferenceException()
        {
            // Arrange
            _fixture.MockMountRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Mount)null!);

            // Act
            var result = async () => await _fixture.MockMountService.GetByIdAsync(_fixture.Id);

            // Assert
            result.Should().NotBeNull();
            await result.Should().ThrowAsync<NullReferenceException>();
        }

        [Fact]
        public void AddAsync_ValidMount_ReturnsVoid()
        {
            // Act
            var result = _fixture.MockMountService.AddAsync(_fixture.Mount);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void UpdateAsync_ValidMount_ReturnsVoid()
        {
            // Act
            var result = _fixture.MockMountService.UpdateAsync(_fixture.Mount);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void DeleteAsync_ValidMount_ReturnsVoid()
        {
            // Act
            var result = _fixture.MockMountService.DeleteAsync(_fixture.Mount);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void AddToCharacter_ValidData_ReturnsVoid()
        {
            // Act
            var result = () => _fixture.MockMountService.AddToCharacter(_fixture.Character, _fixture.Mount);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void RemoveFromCharacter_ExistingCharacterMount_ReturnsVoid()
        {
            // Act
            var result = () => _fixture.MockMountService.RemoveFromCharacter(_fixture.Character, _fixture.Mount);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void RemoveFromCharacter_NonexistingCharacterMount_ThrowsItemNotFoundException()
        {
            // Act
            var result = () => _fixture.MockMountService.RemoveFromCharacter(_fixture.Character, _fixture.Mount);

            // Assert
            result.Should().Throw<ItemNotFoundException>();
        }
    }
}
