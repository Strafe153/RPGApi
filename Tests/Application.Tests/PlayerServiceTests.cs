using Application.Tests.Fixtures;
using Core.Entities;
using Core.Exceptions;
using Core.Models;
using Moq;
using Xunit;

namespace Application.Tests
{
    public class PlayerServiceTests : IClassFixture<PlayerServiceFixture>, IDisposable
    {
        private readonly PlayerServiceFixture _fixture;
        private bool _disposed;

        public PlayerServiceTests(PlayerServiceFixture fixture)
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
                _fixture.MockPlayerRepository.Invocations.Clear();
            }

            _disposed = true;
        }

        [Fact]
        public async Task GetAllAsync_ValidData_ReturnsPagedList()
        {
            // Arrange
            _fixture.MockPlayerRepository
                .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(_fixture.PagedList);

            // Act
            var result = await _fixture.MockPlayerService.GetAllAsync(_fixture.Id, _fixture.Id);

            // Assert
            _fixture.MockPlayerRepository.Verify(r => r.GetAllAsync(_fixture.Id, _fixture.Id), Times.Once());

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.IsType<PaginatedList<Player>>(result);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingPlayer_ReturnsPlayer()
        {
            // Arrange
            _fixture.MockPlayerRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Player);

            // Act
            var result = await _fixture.MockPlayerService.GetByIdAsync(_fixture.Id);

            // Assert
            _fixture.MockPlayerRepository.Verify(r => r.GetByIdAsync(_fixture.Id), Times.Once());

            Assert.NotNull(result);
            Assert.IsType<Player>(result);
        }

        [Fact]
        public async void GetByIdAsync_NonexistingPlayer_ThrowsNullReferenceException()
        {
            // Arrange
            _fixture.MockPlayerRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Player)null!);

            // Act
            var result = _fixture.MockPlayerService.GetByIdAsync(_fixture.Id);

            // Assert
            _fixture.MockPlayerRepository.Verify(r => r.GetByIdAsync(_fixture.Id), Times.Once());

            Assert.True(result.IsFaulted);
            await Assert.ThrowsAsync<NullReferenceException>(async () => await result);
        }

        [Fact]
        public async Task GetByNameAsync_ExistingPlayer_ReturnsPlayer()
        {
            // Arrange
            _fixture.MockPlayerRepository
                .Setup(r => r.GetByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(_fixture.Player);

            // Act
            var result = await _fixture.MockPlayerService.GetByNameAsync(_fixture.Name!);

            // Assert
            _fixture.MockPlayerRepository.Verify(r => r.GetByNameAsync(_fixture.Name!), Times.Once());

            Assert.NotNull(result);
            Assert.IsType<Player>(result);
        }

        [Fact]
        public async Task GetByNameAsync_NonexistingPlayer_ThrowsNullReferenceException()
        {
            // Arrange
            _fixture.MockPlayerRepository
                .Setup(r => r.GetByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((Player)null!);

            // Act
            var result = _fixture.MockPlayerService.GetByNameAsync(_fixture.Name!);

            // Assert
            _fixture.MockPlayerRepository.Verify(r => r.GetByNameAsync(_fixture.Name!), Times.Once());

            Assert.True(result.IsFaulted);
            await Assert.ThrowsAsync<NullReferenceException>(async () => await result);
        }

        [Fact]
        public void AddAsync_ValidPlayer_ReturnsTask()
        {
            // Act
            var result = _fixture.MockPlayerService.AddAsync(_fixture.Player);

            // Assert
            _fixture.MockPlayerRepository.Verify(r => r.Add(_fixture.Player), Times.Once());
            Assert.NotNull(result);
        }

        [Fact]
        public void UpdateAsync_ValidPlayer_ReturnsTask()
        {
            // Act
            var result = _fixture.MockPlayerService.UpdateAsync(_fixture.Player);

            // Assert
            _fixture.MockPlayerRepository.Verify(r => r.Update(_fixture.Player), Times.Once());
            Assert.NotNull(result);
        }

        [Fact]
        public void DeleteAsync_ValidPlayer_ReturnsTask()
        {
            // Act
            var result = _fixture.MockPlayerService.DeleteAsync(_fixture.Player);

            // Assert
            _fixture.MockPlayerRepository.Verify(r => r.Delete(_fixture.Player), Times.Once());
            Assert.NotNull(result);
        }

        [Fact]
        public void VerifyNameUniqueness_UniqueName_ReturnsTask()
        {
            // Arrange
            _fixture.MockPlayerRepository
                .Setup(r => r.GetByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((Player)null!);

            // Act
            var result = _fixture.MockPlayerService.VerifyNameUniqueness(_fixture.Name!);

            // Assert
            _fixture.MockPlayerRepository.Verify(r => r.GetByNameAsync(_fixture.Name!), Times.Once());
            Assert.NotNull(result);
        }

        [Fact]
        public async Task VerifyNameUniqueness_NotUniqueName_ThrowsNameNotUniqueException()
        {
            // Arrange
            _fixture.MockPlayerRepository
                .Setup(r => r.GetByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(_fixture.Player);

            // Act
            var result = _fixture.MockPlayerService.VerifyNameUniqueness(_fixture.Name!);

            // Assert
            _fixture.MockPlayerRepository.Verify(r => r.GetByNameAsync(_fixture.Name!), Times.Once());

            Assert.True(result.IsFaulted);
            await Assert.ThrowsAsync<NameNotUniqueException>(async () => await result);
        }

        [Fact]
        public void CreatePlayer_ValidPlayer_ReturnsPlayer()
        {
            // Act
            var result = _fixture.MockPlayerService.CreatePlayer(_fixture.Name!, _fixture.Bytes, _fixture.Bytes);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Player>(result);
        }

        [Fact]
        public void ChangePasswordData_ValidData_ReturnsVoid()
        {
            // Act
            var result = () => _fixture.MockPlayerService
                .ChangePasswordData(_fixture.Player, _fixture.Bytes, _fixture.Bytes);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void VerifyPlayerAccessRights_SufficientRights_ReturnsVoid()
        {
            // Act
            var result = () => _fixture.MockPlayerService
                .VerifyPlayerAccessRights(_fixture.Player, _fixture.IIdentity, _fixture.SufficientClaims);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void VerifyPlayerAccessRights_InsufficientRights_ReturnsVoid()
        {
            // Act
            var result = () => _fixture.MockPlayerService
                .VerifyPlayerAccessRights(_fixture.Player, _fixture.IIdentity, _fixture.InsufficientClaims);

            // Assert
            Assert.NotNull(result);
            Assert.Throws<NotEnoughRightsException>(result);
        }
    }
}
