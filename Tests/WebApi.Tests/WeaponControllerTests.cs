using Core.Entities;
using Core.Models;
using Core.ViewModels;
using Core.ViewModels.WeaponViewModels;
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
        public async Task GetAsync_ValidPageParameters_ReturnsOkObjectResult()
        {
            // Arrange
            _fixture.MockWeaponService
                .Setup(s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(_fixture.PagedList);

            _fixture.MockPagedMapper
                .Setup(m => m.Map(It.IsAny<PagedList<Weapon>>()))
                .Returns(_fixture.PageViewModel);

            // Act
            var result = await _fixture.MockWeaponsController.GetAsync(_fixture.PageParameters);
            var pageViewModel = (result.Result as OkObjectResult)!.Value as PageViewModel<WeaponReadViewModel>;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ActionResult<PageViewModel<WeaponReadViewModel>>>(result);
            Assert.NotEmpty(pageViewModel!.Entities!);
        }

        [Fact]
        public async Task GetAsync_ValidId_ReturnsOkObjectResult()
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
            var readViewModel = (result.Result as OkObjectResult)!.Value as WeaponReadViewModel;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ActionResult<WeaponReadViewModel>>(result);
            Assert.NotNull(readViewModel);
        }

        [Fact]
        public async Task CreateAsync_ValidViewModel_ReturnsCreatedAtActionResult()
        {
            // Arrange
            _fixture.MockCreateMapper
                .Setup(m => m.Map(It.IsAny<WeaponBaseViewModel>()))
                .Returns(_fixture.Weapon);

            // Act
            var result = await _fixture.MockWeaponsController.CreateAsync(_fixture.SpellBaseViewModel);
            var readViewModel = (result.Result as CreatedAtActionResult)!.Value as WeaponReadViewModel;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ActionResult<WeaponReadViewModel>>(result);
            Assert.NotNull(readViewModel);
        }

        [Fact]
        public async Task UpdateAsync_ValidViewModel_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockWeaponService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Weapon);

            // Act
            var result = await _fixture.MockWeaponsController.UpdateAsync(_fixture.Id, _fixture.SpellBaseViewModel);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateAsync_ValidPatchDocument_ReturnsNoContentResult()
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
            Assert.NotNull(result);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateAsync_InvalidPatchDocument_ReturnsObjectResult()
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
            Assert.NotNull(result);
            Assert.IsType<ObjectResult>(result);
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
            Assert.NotNull(result);
            Assert.IsType<NoContentResult>(result);
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
            Assert.NotNull(result);
            Assert.IsType<NoContentResult>(result);
        }
    }
}
