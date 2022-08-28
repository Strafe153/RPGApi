using Application.Tests.Fixtures;
using Core.Entities;
using Core.Exceptions;
using Core.Models;
using Moq;
using Xunit;

namespace Application.Tests
{
    public class WeaponServiceTests : IClassFixture<WeaponServiceFixture>, IDisposable
    {
        private readonly WeaponServiceFixture _fixture;
        private bool _disposed;

        public WeaponServiceTests(WeaponServiceFixture fixture)
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
                _fixture.MockWeaponRepository.Invocations.Clear();
            }

            _disposed = true;
        }

        [Fact]
        public async Task GetAllAsync_ValidData_ReturnsPagedList()
        {
            // Arrange
            _fixture.MockWeaponRepository
                .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(_fixture.PagedList);

            // Act
            var result = await _fixture.MockWeaponService.GetAllAsync(_fixture.Id, _fixture.Id);

            // Assert
            _fixture.MockWeaponRepository.Verify(r => r.GetAllAsync(_fixture.Id, _fixture.Id), Times.Once());

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.IsType<PaginatedList<Weapon>>(result);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingWeapon_ReturnsWeapon()
        {
            // Arrange
            _fixture.MockWeaponRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Weapon);

            // Act
            var result = await _fixture.MockWeaponService.GetByIdAsync(_fixture.Id);

            // Assert
            _fixture.MockWeaponRepository.Verify(r => r.GetByIdAsync(_fixture.Id), Times.Once());

            Assert.NotNull(result);
            Assert.IsType<Weapon>(result);
        }

        [Fact]
        public async void GetByIdAsync_NonexistingWeapon_ThrowsNullReferenceException()
        {
            // Arrange
            _fixture.MockWeaponRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Weapon)null!);

            // Act
            var result = _fixture.MockWeaponService.GetByIdAsync(_fixture.Id);

            // Assert
            _fixture.MockWeaponRepository.Verify(r => r.GetByIdAsync(_fixture.Id), Times.Once());

            Assert.True(result.IsFaulted);
            await Assert.ThrowsAsync<NullReferenceException>(async () => await result);
        }

        [Fact]
        public void AddAsync_ValidWeapon_ReturnsTask()
        {
            // Act
            var result = _fixture.MockWeaponService.AddAsync(_fixture.Weapon);

            // Assert
            _fixture.MockWeaponRepository.Verify(r => r.Add(_fixture.Weapon), Times.Once());
            Assert.NotNull(result);
        }

        [Fact]
        public void UpdateAsync_ValidWeapon_ReturnsTask()
        {
            // Act
            var result = _fixture.MockWeaponService.UpdateAsync(_fixture.Weapon);

            // Assert
            _fixture.MockWeaponRepository.Verify(r => r.Update(_fixture.Weapon), Times.Once());
            Assert.NotNull(result);
        }

        [Fact]
        public void DeleteAsync_ValidWeapon_ReturnsTask()
        {
            // Act
            var result = _fixture.MockWeaponService.DeleteAsync(_fixture.Weapon);

            // Assert
            _fixture.MockWeaponRepository.Verify(r => r.Delete(_fixture.Weapon), Times.Once());
            Assert.NotNull(result);
        }

        [Fact]
        public void AddToCharacter_ValidData_ReturnsVoid()
        {
            // Act
            var result = () => _fixture.MockWeaponService.AddToCharacter(_fixture.Character, _fixture.Weapon);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void RemoveFromCharacter_ExistingWeapon_ReturnsVoid()
        {
            // Act
            var result = () => _fixture.MockWeaponService.RemoveFromCharacter(_fixture.Character, _fixture.Weapon);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void RemoveFromCharacter_NonexistingWeapon_ThrowsItemNotFoundException()
        {
            // Act
            var result = () => _fixture.MockWeaponService.RemoveFromCharacter(_fixture.Character, _fixture.Weapon);

            // Assert
            Assert.Throws<ItemNotFoundException>(result);
        }
    }
}
