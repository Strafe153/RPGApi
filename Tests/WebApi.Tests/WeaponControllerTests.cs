using Core.Dtos;
using Core.Dtos.WeaponDtos;
using Core.Entities;
using Core.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Tests.Fixtures;
using Xunit;

namespace WebApi.Tests
{
    public class WeaponControllerTests : IClassFixture<WeaponsControllerFixture>
    {
        private readonly WeaponsControllerFixture _fixture;

        public WeaponControllerTests(WeaponsControllerFixture fixture)
        {
            _fixture = fixture;

            _fixture.MockControllerBaseUser();
            _fixture.MockObjectModelValidator(_fixture.MockWeaponsController);
        }

        [Fact]
        public async Task GetAsync_ValidPageParameters_ReturnsActionResultOfPageDtoOfWeaponReadDto()
        {
            // Arrange
            _fixture.MockWeaponService
                .Setup(s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(_fixture.PaginatedList);

            _fixture.MockPaginatedMapper
                .Setup(m => m.Map(It.IsAny<PaginatedList<Weapon>>()))
                .Returns(_fixture.PageDto);

            // Act
            var result = await _fixture.MockWeaponsController.GetAsync(_fixture.PageParameters);
            var pageDto = result.Result.As<OkObjectResult>().Value.As<PageDto<WeaponReadDto>>();

            // Assert
            result.Should().NotBeNull().And.BeOfType<ActionResult<PageDto<WeaponReadDto>>>();
            pageDto.Entities.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetAsync_ExistingWeapon_ReturnsActionResultOfWeaponReadDto()
        {
            // Arrange
            _fixture.MockWeaponService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Weapon);

            _fixture.MockReadMapper
                .Setup(m => m.Map(It.IsAny<Weapon>()))
                .Returns(_fixture.WeaponReadDto);

            // Act
            var result = await _fixture.MockWeaponsController.GetAsync(_fixture.Id);
            var readDto = result.Result.As<OkObjectResult>().Value.As<WeaponReadDto>();

            // Assert
            result.Should().NotBeNull().And.BeOfType<ActionResult<WeaponReadDto>>();
            readDto.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateAsync_ValidDto_ReturnsActionResultOfWeaponReadDto()
        {
            // Arrange
            _fixture.MockCreateMapper
                .Setup(m => m.Map(It.IsAny<WeaponBaseDto>()))
                .Returns(_fixture.Weapon);

            // Act
            var result = await _fixture.MockWeaponsController.CreateAsync(_fixture.WeaponBaseDto);
            var readDto = result.Result.As<CreatedAtActionResult>().Value.As<WeaponReadDto>();

            // Assert
            result.Should().NotBeNull().And.BeOfType<ActionResult<WeaponReadDto>>();
            readDto.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateAsync_ExistingWeaponValidDto_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockWeaponService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Weapon);

            // Act
            var result = await _fixture.MockWeaponsController.UpdateAsync(_fixture.Id, _fixture.WeaponBaseDto);

            // Assert
            result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UpdateAsync_ExistingWeaponValidPatchDocument_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockWeaponService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Weapon);

            _fixture.MockUpdateMapper
                .Setup(m => m.Map(_fixture.Weapon))
                .Returns(_fixture.WeaponBaseDto);

            // Act
            var result = await _fixture.MockWeaponsController.UpdateAsync(_fixture.Id, _fixture.PatchDocument);

            // Assert
            result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UpdateAsync_ExistingWeaponInvalidPatchDocument_ReturnsObjectResult()
        {
            // Arrange
            _fixture.MockWeaponService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Weapon);

            _fixture.MockUpdateMapper
                .Setup(m => m.Map(_fixture.Weapon))
                .Returns(_fixture.WeaponBaseDto);

            _fixture.MockModelError(_fixture.MockWeaponsController);

            // Act
            var result = await _fixture.MockWeaponsController.UpdateAsync(_fixture.Id, _fixture.PatchDocument);

            // Assert
            result.Should().NotBeNull().And.BeOfType<ObjectResult>();
        }

        [Fact]
        public async Task DeleteAsync_ExistingWeapon_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockWeaponService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Weapon);

            // Act
            var result = await _fixture.MockWeaponsController.DeleteAsync(_fixture.Id);

            // Assert
            result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task HitAsync_ValidDto_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockCharacterService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Character);

            _fixture.MockCharacterService
                .Setup(s => s.GetWeapon(It.IsAny<Character>(), It.IsAny<int>()))
                .Returns(_fixture.Weapon);

            // Act
            var result = await _fixture.MockWeaponsController.HitAsync(_fixture.HitDto);

            // Assert
            result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        }
    }
}
