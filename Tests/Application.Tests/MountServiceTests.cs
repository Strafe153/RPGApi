using Application.Tests.Fixtures;
using Core.Entities;
using Core.Exceptions;
using Core.Models;
using Moq;
using Xunit;

namespace Application.Tests
{
    public class MountServiceTests : IClassFixture<MountServiceFixture>, IDisposable
    {
        private readonly MountServiceFixture _fixture;
        private bool _disposed;

        public MountServiceTests(MountServiceFixture fixture)
        {
            _fixture = fixture;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _fixture.MockMountRepository.Invocations.Clear();
            }

            _disposed = true;
        }

        [Fact]
        public async Task GetAllAsync_ValidData_ReturnsPagedList()
        {
            // Arrange
            _fixture.MockMountRepository
                .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(_fixture.PagedList);

            // Act
            var result = await _fixture.MockMountService.GetAllAsync(_fixture.Id, _fixture.Id);

            // Assert
            _fixture.MockMountRepository.Verify(r => r.GetAllAsync(_fixture.Id, _fixture.Id), Times.Once());

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.IsType<PaginatedList<Mount>>(result);
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
            _fixture.MockMountRepository.Verify(r => r.GetByIdAsync(_fixture.Id), Times.Once());

            Assert.NotNull(result);
            Assert.IsType<Mount>(result);
        }

        [Fact]
        public async void GetByIdAsync_NonexistingMount_ThrowsNullReferenceException()
        {
            // Arrange
            _fixture.MockMountRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Mount)null!);

            // Act
            var result = _fixture.MockMountService.GetByIdAsync(_fixture.Id);

            // Assert
            _fixture.MockMountRepository.Verify(r => r.GetByIdAsync(_fixture.Id), Times.Once());

            Assert.True(result.IsFaulted);
            await Assert.ThrowsAsync<NullReferenceException>(async () => await result);
        }

        [Fact]
        public void AddAsync_ValidMount_ReturnsTask()
        {
            // Act
            var result = _fixture.MockMountService.AddAsync(_fixture.Mount);

            // Assert
            _fixture.MockMountRepository.Verify(r => r.Add(_fixture.Mount), Times.Once());
            Assert.NotNull(result);
        }

        [Fact]
        public void UpdateAsync_ValidMount_ReturnsTask()
        {
            // Act
            var result = _fixture.MockMountService.UpdateAsync(_fixture.Mount);

            // Assert
            _fixture.MockMountRepository.Verify(r => r.Update(_fixture.Mount), Times.Once());
            Assert.NotNull(result);
        }

        [Fact]
        public void DeleteAsync_ValidMount_ReturnsTask()
        {
            // Act
            var result = _fixture.MockMountService.DeleteAsync(_fixture.Mount);

            // Assert
            _fixture.MockMountRepository.Verify(r => r.Delete(_fixture.Mount), Times.Once());
            Assert.NotNull(result);
        }

        [Fact]
        public void AddToCharacter_ValidData_ReturnsVoid()
        {
            // Act
            var result = () => _fixture.MockMountService.AddToCharacter(_fixture.Character, _fixture.Mount);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void RemoveFromCharacter_ExistingMount_ReturnsVoid()
        {
            // Act
            var result = () => _fixture.MockMountService.RemoveFromCharacter(_fixture.Character, _fixture.Mount);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void RemoveFromCharacter_NonexistingMount_ThrowsItemNotFoundException()
        {
            // Act
            var result = () => _fixture.MockMountService.RemoveFromCharacter(_fixture.Character, _fixture.Mount);

            // Assert
            Assert.Throws<ItemNotFoundException>(result);
        }
    }
}
