using Application.Tests.Fixtures;
using Core.Entities;
using Core.Exceptions;
using Core.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.Tests
{
    public class WeaponServiceTests : IClassFixture<WeaponServiceFixture>
    {
        private readonly WeaponServiceFixture _fixture;

        public WeaponServiceTests(WeaponServiceFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetAllAsync_ValidParameters_ReturnsPaginatedList()
        {
            // Arrange
            _fixture.MockWeaponRepository
                .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(_fixture.PaginatedList);

            // Act
            var result = await _fixture.MockWeaponService.GetAllAsync(_fixture.Id, _fixture.Id);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Should().BeOfType<PaginatedList<Weapon>>();
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
            result.Should().NotBeNull();
            result.Should().BeOfType<Weapon>();
        }

        [Fact]
        public async void GetByIdAsync_NonexistingWeapon_ThrowsNullReferenceException()
        {
            // Arrange
            _fixture.MockWeaponRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Weapon)null!);

            // Act
            var result = async () => await _fixture.MockWeaponService.GetByIdAsync(_fixture.Id);

            // Assert
            result.Should().NotBeNull();
            await result.Should().ThrowAsync<NullReferenceException>();
        }

        [Fact]
        public void AddAsync_ValidWeapon_ReturnsVoid()
        {
            // Act
            var result = _fixture.MockWeaponService.AddAsync(_fixture.Weapon);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void UpdateAsync_ValidWeapon_ReturnsVoid()
        {
            // Act
            var result = _fixture.MockWeaponService.UpdateAsync(_fixture.Weapon);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void DeleteAsync_ValidWeapon_ReturnsVoid()
        {
            // Act
            var result = _fixture.MockWeaponService.DeleteAsync(_fixture.Weapon);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void AddToCharacter_ValidData_ReturnsVoid()
        {
            // Act
            var result = () => _fixture.MockWeaponService.AddToCharacter(_fixture.Character, _fixture.Weapon);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void RemoveFromCharacter_ExistingWeapon_ReturnsVoid()
        {
            // Act
            var result = () => _fixture.MockWeaponService.RemoveFromCharacter(_fixture.Character, _fixture.Weapon);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void RemoveFromCharacter_NonexistingWeapon_ThrowsItemNotFoundException()
        {
            // Act
            var result = () => _fixture.MockWeaponService.RemoveFromCharacter(_fixture.Character, _fixture.Weapon);

            // Assert
            result.Should().Throw<ItemNotFoundException>();
        }
    }
}
