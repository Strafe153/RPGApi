using Application.Tests.Fixtures;
using Core.Entities;
using Core.Exceptions;
using Core.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.Tests
{
    public class CharacterServiceTests : IClassFixture<CharacterServiceFixture>
    {
        private readonly CharacterServiceFixture _fixture;

        public CharacterServiceTests(CharacterServiceFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetAllAsync_ValidParameters_ReturnsPaginatedListOfCharacter()
        {
            // Arrange
            _fixture.MockCharacterRepository
                .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(_fixture.PaginatedList);

            // Act
            var result = await _fixture.MockCharacterService.GetAllAsync(_fixture.Id, _fixture.Id);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Should().BeOfType<PaginatedList<Character>>();
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
            result.Should().NotBeNull();
            result.Should().BeOfType<Character>();
        }

        [Fact]
        public async Task GetByIdAsync_NonexistingCharacter_ThrowsNullReferenceException()
        {
            // Arrange
            _fixture.MockCharacterRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Character)null!);

            // Act
            var result = async () => await _fixture.MockCharacterService.GetByIdAsync(_fixture.Id);

            // Assert
            result.Should().NotBeNull();
            await result.Should().ThrowAsync<NullReferenceException>();
        }

        [Fact]
        public void AddAsync_ValidCharacter_ReturnsTask()
        {
            // Act
            var result = _fixture.MockCharacterService.AddAsync(_fixture.Character);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void UpdateAsync_ValidCharacter_ReturnsTask()
        {
            // Act
            var result = _fixture.MockCharacterService.UpdateAsync(_fixture.Character);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void DeleteAsync_ValidCharacter_ReturnsTask()
        {
            // Act
            var result = _fixture.MockCharacterService.DeleteAsync(_fixture.Character);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void GetWeapon_ExistingWeapon_ReturnsWeapon()
        {
            // Act
            var result = _fixture.MockCharacterService.GetWeapon(_fixture.Character, _fixture.Id);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Weapon>();
        }

        [Fact]
        public void GetWeapon_NonexistingWeapon_ThrowsItemNotFoundException()
        {
            // Act
            var result = () => _fixture.MockCharacterService.GetWeapon(_fixture.Character, 3);

            // Assert
            result.Should().Throw<ItemNotFoundException>();
        }

        [Fact]
        public void GetSpell_ExistingSpell_ReturnsSpell()
        {
            // Act
            var result = _fixture.MockCharacterService.GetSpell(_fixture.Character, _fixture.Id);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Spell>();
        }

        [Fact]
        public void GetSpell_NonexistingSpell_ThrowsItemNotFoundException()
        {
            // Act
            var result = () => _fixture.MockCharacterService.GetSpell(_fixture.Character, 3);

            // Assert
            result.Should().Throw<ItemNotFoundException>();
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
            resultingHealth.Should().Be(_fixture.Character.Health);
        }
    }
}
