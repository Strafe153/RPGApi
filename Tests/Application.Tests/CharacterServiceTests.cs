using Application.Tests.Fixtures;
using Core.Entities;
using Core.Exceptions;
using Core.Models;
using Moq;
using Xunit;

namespace Application.Tests
{
    public class CharacterServiceTests : IClassFixture<CharacterServiceFixture>, IDisposable
    {
        private readonly CharacterServiceFixture _fixture;
        private bool _disposed;

        public CharacterServiceTests(CharacterServiceFixture fixture)
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
                _fixture.MockCharacterRepository.Invocations.Clear();
            }

            _disposed = true;
        }

        [Fact]
        public async Task GetAllAsync_ValidData_ReturnsPagedList()
        {
            // Arrange
            _fixture.MockCharacterRepository
                .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(_fixture.PagedList);

            // Act
            var result = await _fixture.MockCharacterService.GetAllAsync(_fixture.Id, _fixture.Id);

            // Assert
            _fixture.MockCharacterRepository.Verify(r => r.GetAllAsync(_fixture.Id, _fixture.Id), Times.Once());

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.IsType<PagedList<Character>>(result);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingCharacter_ReturnsCharacter()
        {
            // Arrange
            _fixture.MockCharacterRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Character);

            // Act
            var result = await _fixture.MockCharacterService.GetByIdAsync(_fixture.Id);

            // Assert
            _fixture.MockCharacterRepository.Verify(r => r.GetByIdAsync(_fixture.Id), Times.Once());

            Assert.NotNull(result);
            Assert.IsType<Character>(result);
        }

        [Fact]
        public async Task GetByIdAsync_NonexistingCharacter_ThrowsNullReferenceException()
        {
            // Arrange
            _fixture.MockCharacterRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Character)null!);

            // Act
            var result = _fixture.MockCharacterService.GetByIdAsync(_fixture.Id);

            // Assert
            _fixture.MockCharacterRepository.Verify(r => r.GetByIdAsync(_fixture.Id), Times.Once());

            Assert.True(result.IsFaulted);
            await Assert.ThrowsAsync<NullReferenceException>(async () => await result);
        }

        [Fact]
        public void AddAsync_ValidCharacter_ReturnsTask()
        {
            // Act
            var result = _fixture.MockCharacterService.AddAsync(_fixture.Character);

            // Assert
            _fixture.MockCharacterRepository.Verify(r => r.Add(_fixture.Character), Times.Once());
            Assert.NotNull(result);
        }

        [Fact]
        public void UpdateAsync_ValidCharacter_ReturnsTask()
        {
            // Act
            var result = _fixture.MockCharacterService.UpdateAsync(_fixture.Character);

            // Assert
            _fixture.MockCharacterRepository.Verify(r => r.Update(_fixture.Character), Times.Once());
            Assert.NotNull(result);
        }

        [Fact]
        public void DeleteAsync_ValidCharacter_ReturnsTask()
        {
            // Act
            var result = _fixture.MockCharacterService.DeleteAsync(_fixture.Character);

            // Assert
            _fixture.MockCharacterRepository.Verify(r => r.Delete(_fixture.Character), Times.Once());
            Assert.NotNull(result);
        }

        [Fact]
        public void GetWeapon_ExistingWeapon_ReturnsWeapon()
        {
            // Act
            var result = _fixture.MockCharacterService.GetWeapon(_fixture.Character, _fixture.Id);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Weapon>(result);
        }

        [Fact]
        public void GetWeapon_NonexistingWeapon_ThrowsItemNotFoundException()
        {
            // Act
            var result = () => _fixture.MockCharacterService.GetWeapon(_fixture.Character, 3);

            // Assert
            Assert.Throws<ItemNotFoundException>(result);
        }

        [Fact]
        public void GetSpell_ExistingSpell_ReturnsWeapon()
        {
            // Act
            var result = _fixture.MockCharacterService.GetSpell(_fixture.Character, _fixture.Id);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Spell>(result);
        }

        [Fact]
        public void GetSpell_NonexistingSpell_ThrowsItemNotFoundException()
        {
            // Act
            var result = () => _fixture.MockCharacterService.GetSpell(_fixture.Character, 3);

            // Assert
            Assert.Throws<ItemNotFoundException>(result);
        }

        [Theory]
        [InlineData(20, 80)]
        [InlineData(100, 0)]
        [InlineData(-25, 100)]
        public void CalculateHealth_VariousDamage_ReturnsRemainingHealth(int damage, int resultingHealth)
        {
            // Act
            _fixture.MockCharacterService.CalculateHealth(_fixture.Character, damage);

            // Assert
            Assert.Equal(resultingHealth, _fixture.Character.Health);
        }
    }
}
