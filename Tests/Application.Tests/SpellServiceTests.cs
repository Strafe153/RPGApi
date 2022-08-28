using Application.Tests.Fixtures;
using Core.Entities;
using Core.Exceptions;
using Core.Models;
using Moq;
using Xunit;

namespace Application.Tests
{
    public class SpellServiceTests : IClassFixture<SpellServiceFixture>, IDisposable
    {
        private readonly SpellServiceFixture _fixture;
        private bool _disposed;

        public SpellServiceTests(SpellServiceFixture fixture)
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
                _fixture.MockSpellRepository.Invocations.Clear();
            }

            _disposed = true;
        }

        [Fact]
        public async Task GetAllAsync_ValidData_ReturnsPagedList()
        {
            // Arrange
            _fixture.MockSpellRepository
                .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(_fixture.PagedList);

            // Act
            var result = await _fixture.MockSpellService.GetAllAsync(_fixture.Id, _fixture.Id);

            // Assert
            _fixture.MockSpellRepository.Verify(r => r.GetAllAsync(_fixture.Id, _fixture.Id), Times.Once());

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.IsType<PaginatedList<Spell>>(result);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingSpell_ReturnsSpell()
        {
            // Arrange
            _fixture.MockSpellRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Spell);

            // Act
            var result = await _fixture.MockSpellService.GetByIdAsync(_fixture.Id);

            // Assert
            _fixture.MockSpellRepository.Verify(r => r.GetByIdAsync(_fixture.Id), Times.Once());

            Assert.NotNull(result);
            Assert.IsType<Spell>(result);
        }

        [Fact]
        public async void GetByIdAsync_NonexistingSpell_ThrowsNullReferenceException()
        {
            // Arrange
            _fixture.MockSpellRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Spell)null!);

            // Act
            var result = _fixture.MockSpellService.GetByIdAsync(_fixture.Id);

            // Assert
            _fixture.MockSpellRepository.Verify(r => r.GetByIdAsync(_fixture.Id), Times.Once());

            Assert.True(result.IsFaulted);
            await Assert.ThrowsAsync<NullReferenceException>(async () => await result);
        }

        [Fact]
        public void AddAsync_ValidSpell_ReturnsTask()
        {
            // Act
            var result = _fixture.MockSpellService.AddAsync(_fixture.Spell);

            // Assert
            _fixture.MockSpellRepository.Verify(r => r.Add(_fixture.Spell), Times.Once());
            Assert.NotNull(result);
        }

        [Fact]
        public void UpdateAsync_ValidSpell_ReturnsTask()
        {
            // Act
            var result = _fixture.MockSpellService.UpdateAsync(_fixture.Spell);

            // Assert
            _fixture.MockSpellRepository.Verify(r => r.Update(_fixture.Spell), Times.Once());
            Assert.NotNull(result);
        }

        [Fact]
        public void DeleteAsync_ValidSpell_ReturnsTask()
        {
            // Act
            var result = _fixture.MockSpellService.DeleteAsync(_fixture.Spell);

            // Assert
            _fixture.MockSpellRepository.Verify(r => r.Delete(_fixture.Spell), Times.Once());
            Assert.NotNull(result);
        }

        [Fact]
        public void AddToCharacter_ValidData_ReturnsVoid()
        {
            // Act
            var result = () => _fixture.MockSpellService.AddToCharacter(_fixture.Character, _fixture.Spell);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void RemoveFromCharacter_ExistingSpell_ReturnsVoid()
        {
            // Act
            var result = () => _fixture.MockSpellService.RemoveFromCharacter(_fixture.Character, _fixture.Spell);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void RemoveFromCharacter_NonexistingSpell_ThrowsItemNotFoundException()
        {
            // Act
            var result = () => _fixture.MockSpellService.RemoveFromCharacter(_fixture.Character, _fixture.Spell);

            // Assert
            Assert.Throws<ItemNotFoundException>(result);
        }
    }
}
