using Application.Tests.Fixtures;
using Core.Entities;
using Core.Exceptions;
using Core.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.Tests
{
    public class SpellServiceTests : IClassFixture<SpellServiceFixture>
    {
        private readonly SpellServiceFixture _fixture;

        public SpellServiceTests(SpellServiceFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetAllAsync_ValidParameters_ReturnsPaginatedListOfSpell()
        {
            // Arrange
            _fixture.MockSpellRepository
                .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(_fixture.PaginatedList);

            // Act
            var result = await _fixture.MockSpellService.GetAllAsync(_fixture.Id, _fixture.Id);

            // Assert
            result.Should().NotBeNull().And.NotBeEmpty().And.BeOfType<PaginatedList<Spell>>();
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
            result.Should().NotBeNull().And.BeOfType<Spell>();
        }

        [Fact]
        public async Task GetByIdAsync_NonexistingSpell_ThrowsNullReferenceException()
        {
            // Arrange
            _fixture.MockSpellRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Spell)null!);

            // Act
            var result = async () => await _fixture.MockSpellService.GetByIdAsync(_fixture.Id);

            // Assert
            await result.Should().ThrowAsync<NullReferenceException>();
        }

        [Fact]
        public void AddAsync_ValidSpell_ReturnsTask()
        {
            // Act
            var result = _fixture.MockSpellService.AddAsync(_fixture.Spell);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void UpdateAsync_ValidSpell_ReturnsTask()
        {
            // Act
            var result = _fixture.MockSpellService.UpdateAsync(_fixture.Spell);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void DeleteAsync_ValidSpell_ReturnsTask()
        {
            // Act
            var result = _fixture.MockSpellService.DeleteAsync(_fixture.Spell);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void AddToCharacter_ValidData_ReturnsVoid()
        {
            // Act
            var result = () => _fixture.MockSpellService.AddToCharacter(_fixture.Character, _fixture.Spell);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void RemoveFromCharacter_ExistingSpell_ReturnsVoid()
        {
            // Act
            var result = () => _fixture.MockSpellService.RemoveFromCharacter(_fixture.Character, _fixture.Spell);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void RemoveFromCharacter_NonexistingSpell_ThrowsItemNotFoundException()
        {
            // Act
            var result = () => _fixture.MockSpellService.RemoveFromCharacter(_fixture.Character, _fixture.Spell);

            // Assert
            result.Should().Throw<ItemNotFoundException>();
        }
    }
}
