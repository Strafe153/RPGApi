using Core.Dtos;
using Core.Dtos.WeaponDtos;
using Core.Entities;
using Core.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using WebApi.Tests.Fixtures;

namespace WebApi.Tests
{
    [TestFixture]
    public class WeaponControllerTests
    {
        private WeaponsControllerFixture _fixture = default!;

        [OneTimeSetUp]
        public void SetUp()
        {
            _fixture = new WeaponsControllerFixture();

            _fixture.MockControllerBaseUser();
            _fixture.MockObjectModelValidator(_fixture.WeaponsController);
        }

        [TearDown]
        public void TearDown()
        {
            _fixture.WeaponsController.ControllerContext = _fixture.MockControllerContext();
        }

        [Test]
        public async Task GetAsync_ValidPageParameters_ReturnsActionResultOfPageDtoOfWeaponReadDto()
        {
            // Arrange
            _fixture.WeaponService
                .GetAllAsync(Arg.Any<int>(), Arg.Any<int>())
                .Returns(_fixture.PaginatedList);

            _fixture.PaginatedMapper
                .Map(Arg.Any<PaginatedList<Weapon>>())
                .Returns(_fixture.PageDto);

            // Act
            var result = await _fixture.WeaponsController.GetAsync(_fixture.PageParameters);
            var pageDto = result.Result.As<OkObjectResult>().Value.As<PageDto<WeaponReadDto>>();

            // Assert
            result.Should().NotBeNull().And.BeOfType<ActionResult<PageDto<WeaponReadDto>>>();
            pageDto.Entities.Should().NotBeEmpty();
        }

        [Test]
        public async Task GetAsync_ExistingWeapon_ReturnsActionResultOfWeaponReadDto()
        {
            // Arrange
            _fixture.WeaponService
                .GetByIdAsync(Arg.Any<int>())
                .Returns(_fixture.Weapon);

            _fixture.ReadMapper
                .Map(Arg.Any<Weapon>())
                .Returns(_fixture.WeaponReadDto);

            // Act
            var result = await _fixture.WeaponsController.GetAsync(_fixture.Id);
            var readDto = result.Result.As<OkObjectResult>().Value.As<WeaponReadDto>();

            // Assert
            result.Should().NotBeNull().And.BeOfType<ActionResult<WeaponReadDto>>();
            readDto.Should().NotBeNull();
        }

        [Test]
        public async Task CreateAsync_ValidDto_ReturnsActionResultOfWeaponReadDto()
        {
            // Arrange
            _fixture.CreateMapper
                .Map(Arg.Any<WeaponBaseDto>())
                .Returns(_fixture.Weapon);

            _fixture.ReadMapper
                .Map(Arg.Any<Weapon>())
                .Returns(_fixture.WeaponReadDto);

            // Act
            var result = await _fixture.WeaponsController.CreateAsync(_fixture.WeaponBaseDto);
            var readDto = result.Result.As<CreatedAtActionResult>().Value.As<WeaponReadDto>();

            // Assert
            result.Should().NotBeNull().And.BeOfType<ActionResult<WeaponReadDto>>();
            readDto.Should().NotBeNull();
        }

        [Test]
        public async Task UpdateAsync_ExistingWeaponValidDto_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.WeaponService
                .GetByIdAsync(Arg.Any<int>())
                .Returns(_fixture.Weapon);

            // Act
            var result = await _fixture.WeaponsController.UpdateAsync(_fixture.Id, _fixture.WeaponBaseDto);

            // Assert
            result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        }

        [Test]
        public async Task UpdateAsync_ExistingWeaponValidPatchDocument_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.WeaponService
                .GetByIdAsync(Arg.Any<int>())
                .Returns(_fixture.Weapon);

            _fixture.UpdateMapper
                .Map(Arg.Any<Weapon>())
                .Returns(_fixture.WeaponBaseDto);

            // Act
            var result = await _fixture.WeaponsController.UpdateAsync(_fixture.Id, _fixture.PatchDocument);

            // Assert
            result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        }

        [Test]
        public async Task UpdateAsync_ExistingWeaponInvalidPatchDocument_ReturnsObjectResult()
        {
            // Arrange
            _fixture.WeaponService
                .GetByIdAsync(Arg.Any<int>())
                .Returns(_fixture.Weapon);

            _fixture.UpdateMapper
                .Map(Arg.Any<Weapon>())
                .Returns(_fixture.WeaponBaseDto);

            _fixture.MockModelError(_fixture.WeaponsController);

            // Act
            var result = await _fixture.WeaponsController.UpdateAsync(_fixture.Id, _fixture.PatchDocument);

            // Assert
            result.Should().NotBeNull().And.BeOfType<ObjectResult>();
        }

        [Test]
        public async Task DeleteAsync_ExistingWeapon_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.WeaponService
                .GetByIdAsync(Arg.Any<int>())
                .Returns(_fixture.Weapon);

            // Act
            var result = await _fixture.WeaponsController.DeleteAsync(_fixture.Id);

            // Assert
            result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        }

        [Test]
        public async Task HitAsync_ValidDto_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.CharacterService
                .GetByIdAsync(Arg.Any<int>())
                .Returns(_fixture.Character);

            _fixture.CharacterService
                .GetWeapon(Arg.Any<Character>(), Arg.Any<int>())
                .Returns(_fixture.Weapon);

            // Act
            var result = await _fixture.WeaponsController.HitAsync(_fixture.HitDto);

            // Assert
            result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        }
    }
}
