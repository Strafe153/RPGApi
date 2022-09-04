using Application.Tests.Fixtures;
using Core.Entities;
using Core.Exceptions;
using Core.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.Tests
{
    public class PlayerServiceTests : IClassFixture<PlayerServiceFixture>
    {
        private readonly PlayerServiceFixture _fixture;

        public PlayerServiceTests(PlayerServiceFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetAllAsync_ValidParameters_ReturnsPaginatedListOfPlayer()
        {
            // Arrange
            _fixture.MockPlayerRepository
                .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(_fixture.PaginatedList);

            // Act
            var result = await _fixture.MockPlayerService.GetAllAsync(_fixture.Id, _fixture.Id);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Should().BeOfType<PaginatedList<Player>>();
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
            result.Should().NotBeNull();
            result.Should().BeOfType<Player>();
        }

        [Fact]
        public async Task GetByIdAsync_NonexistingPlayer_ThrowsNullReferenceException()
        {
            // Arrange
            _fixture.MockPlayerRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Player)null!);

            // Act
            var result = async () => await _fixture.MockPlayerService.GetByIdAsync(_fixture.Id);

            // Assert
            result.Should().NotBeNull();
            await result.Should().ThrowAsync<NullReferenceException>();
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
            result.Should().NotBeNull();
            result.Should().BeOfType<Player>();
        }

        [Fact]
        public async Task GetByNameAsync_NonexistingPlayer_ThrowsNullReferenceException()
        {
            // Arrange
            _fixture.MockPlayerRepository
                .Setup(r => r.GetByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((Player)null!);

            // Act
            var result = async () => await _fixture.MockPlayerService.GetByNameAsync(_fixture.Name!);

            // Assert
            result.Should().NotBeNull();
            await result.Should().ThrowAsync<NullReferenceException>();
        }

        [Fact]
        public void AddAsync_ValidPlayer_ReturnsTask()
        {
            // Act
            var result = _fixture.MockPlayerService.AddAsync(_fixture.Player);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void UpdateAsync_ValidPlayer_ReturnsTask()
        {
            // Act
            var result = _fixture.MockPlayerService.UpdateAsync(_fixture.Player);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void DeleteAsync_ValidPlayer_ReturnsTask()
        {
            // Act
            var result = _fixture.MockPlayerService.DeleteAsync(_fixture.Player);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void CreatePlayer_ValidPlayer_ReturnsPlayer()
        {
            // Act
            var result = _fixture.MockPlayerService.CreatePlayer(_fixture.Name!, _fixture.Bytes, _fixture.Bytes);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Player>();
        }

        [Fact]
        public void ChangePasswordData_ValidData_ReturnsVoid()
        {
            // Act
            var result = () => _fixture.MockPlayerService
                .ChangePasswordData(_fixture.Player, _fixture.Bytes, _fixture.Bytes);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void VerifyPlayerAccessRights_SufficientRights_ReturnsVoid()
        {
            // Act
            var result = () => _fixture.MockPlayerService
                .VerifyPlayerAccessRights(_fixture.Player, _fixture.IIdentity, _fixture.SufficientClaims);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void VerifyPlayerAccessRights_InsufficientRights_ReturnsVoid()
        {
            // Act
            var result = () => _fixture.MockPlayerService
                .VerifyPlayerAccessRights(_fixture.Player, _fixture.IIdentity, _fixture.InsufficientClaims);

            // Assert
            result.Should().NotBeNull();
            result.Should().Throw<NotEnoughRightsException>();
        }
    }
}
