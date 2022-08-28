using Core.Entities;
using Core.Models;
using Core.ViewModels;
using Core.ViewModels.WeaponViewModels;
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
        public async Task GetAsync_ValidPageParameters_ReturnsActionResultOfPageViewModelOfWeaponReadViewModel()
        {
            // Arrange
            _fixture.MockWeaponService
                .Setup(s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(_fixture.PagedList);

            _fixture.MockPaginatedMapper
                .Setup(m => m.Map(It.IsAny<PaginatedList<Weapon>>()))
                .Returns(_fixture.PageViewModel);

            // Act
            var result = await _fixture.MockWeaponsController.GetAsync(_fixture.PageParameters);
            var pageViewModel = result.Result.As<OkObjectResult>().Value.As<PageViewModel<WeaponReadViewModel>>();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<PageViewModel<WeaponReadViewModel>>>();
            pageViewModel.Entities.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetAsync_ExistingWeapon_ReturnsActionResultOfWeaponReadViewModel()
        {
            // Arrange
            _fixture.MockWeaponService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Weapon);

            _fixture.MockReadMapper
                .Setup(m => m.Map(It.IsAny<Weapon>()))
                .Returns(_fixture.WeaponReadViewModel);

            // Act
            var result = await _fixture.MockWeaponsController.GetAsync(_fixture.Id);
            var readViewModel = result.Result.As<OkObjectResult>().Value.As<WeaponReadViewModel>();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<WeaponReadViewModel>>();
            readViewModel.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateAsync_ValidViewModel_ReturnsActionResultOfWeaponReadViewModel()
        {
            // Arrange
            _fixture.MockCreateMapper
                .Setup(m => m.Map(It.IsAny<WeaponBaseViewModel>()))
                .Returns(_fixture.Weapon);

            // Act
            var result = await _fixture.MockWeaponsController.CreateAsync(_fixture.SpellBaseViewModel);
            var readViewModel = result.Result.As<CreatedAtActionResult>().Value.As<WeaponReadViewModel>();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<WeaponReadViewModel>>();
            readViewModel.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateAsync_ExistingWeaponValidViewModel_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockWeaponService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Weapon);

            // Act
            var result = await _fixture.MockWeaponsController.UpdateAsync(_fixture.Id, _fixture.SpellBaseViewModel);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NoContentResult>();
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
                .Returns(_fixture.SpellBaseViewModel);

            // Act
            var result = await _fixture.MockWeaponsController.UpdateAsync(_fixture.Id, _fixture.PatchDocument);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NoContentResult>();
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
                .Returns(_fixture.SpellBaseViewModel);

            _fixture.MockModelError(_fixture.MockWeaponsController);

            // Act
            var result = await _fixture.MockWeaponsController.UpdateAsync(_fixture.Id, _fixture.PatchDocument);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ObjectResult>();
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
            result.Should().NotBeNull();
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task HitAsync_ValidViewModel_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockCharacterService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Character);

            _fixture.MockCharacterService
                .Setup(s => s.GetWeapon(It.IsAny<Character>(), It.IsAny<int>()))
                .Returns(_fixture.Weapon);

            // Act
            var result = await _fixture.MockWeaponsController.HitAsync(_fixture.HitViewModel);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NoContentResult>();
        }
    }
}
