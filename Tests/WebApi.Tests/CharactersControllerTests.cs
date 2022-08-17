using Core.Entities;
using Core.Models;
using Core.ViewModels;
using Core.ViewModels.CharacterViewModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Tests.Fixtures;
using Xunit;

namespace WebApi.Tests
{
    public class CharactersControllerTests : IClassFixture<CharactersControllerFixture>
    {
        private readonly CharactersControllerFixture _fixture;

        public CharactersControllerTests(CharactersControllerFixture fixture)
        {
            _fixture = fixture;

            _fixture.MockControllerBaseUser();
            _fixture.MockObjectModelValidator(_fixture.MockCharactersController);
        }

        [Fact]
        public async Task GetAsync_ValidPageParameters_ReturnsOkObjectResult()
        {
            // Arrange
            _fixture.MockCharacterService
                .Setup(s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(_fixture.PagedList);

            _fixture.MockPagedMapper
                .Setup(m => m.Map(It.IsAny<PagedList<Character>>()))
                .Returns(_fixture.PageViewModel);

            // Act
            var result = await _fixture.MockCharactersController.GetAsync(_fixture.PageParameters);
            var pageViewModel = (result.Result as OkObjectResult)!.Value as PageViewModel<CharacterReadViewModel>;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ActionResult<PageViewModel<CharacterReadViewModel>>>(result);
            Assert.NotEmpty(pageViewModel!.Entities!);
        }

        [Fact]
        public async Task GetAsync_ValidId_ReturnsOkObjectResult()
        {
            // Arrange
            _fixture.MockCharacterService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Character);

            _fixture.MockReadMapper
                .Setup(m => m.Map(It.IsAny<Character>()))
                .Returns(_fixture.CharacterReadViewModel);

            // Act
            var result = await _fixture.MockCharactersController.GetAsync(_fixture.Id);
            var readViewModel = (result.Result as OkObjectResult)!.Value as CharacterReadViewModel;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ActionResult<CharacterReadViewModel>>(result);
            Assert.NotNull(readViewModel);
        }

        [Fact]
        public async Task CreateAsync_ValidViewModel_ReturnsCreatedAtActionResult()
        {
            // Arrange
            _fixture.MockCreateMapper
                .Setup(m => m.Map(It.IsAny<CharacterCreateViewModel>()))
                .Returns(_fixture.Character);

            // Act
            var result = await _fixture.MockCharactersController.CreateAsync(_fixture.CharacterCreateViewModel);
            var readViewModel = (result.Result as CreatedAtActionResult)!.Value as CharacterReadViewModel;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ActionResult<CharacterReadViewModel>>(result);
            Assert.NotNull(readViewModel);
        }

        [Fact]
        public async Task UpdateAsync_ValidViewModel_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockCharacterService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Character);

            // Act
            var result = await _fixture.MockCharactersController
                .UpdateAsync(_fixture.Id, _fixture.CharacterUpdateViewModel);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateAsync_ValidPatchDocument_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockCharacterService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Character);

            _fixture.MockUpdateMapper
                .Setup(m => m.Map(_fixture.Character))
                .Returns(_fixture.CharacterUpdateViewModel);

            // Act
            var result = await _fixture.MockCharactersController.UpdateAsync(_fixture.Id, _fixture.PatchDocument);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateAsync_InvalidPatchDocument_ReturnsObjectResult()
        {
            // Arrange
            _fixture.MockCharacterService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Character);

            _fixture.MockUpdateMapper
                .Setup(m => m.Map(_fixture.Character))
                .Returns(_fixture.CharacterUpdateViewModel);

            _fixture.MockModelError(_fixture.MockCharactersController);

            // Act
            var result = await _fixture.MockCharactersController.UpdateAsync(_fixture.Id, _fixture.PatchDocument);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ObjectResult>(result);
        }

        [Fact]
        public async Task DeleteAsync_ExistingCharacter_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockCharacterService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Character);

            // Act
            var result = await _fixture.MockCharactersController.DeleteAsync(_fixture.Id);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task AddWeaponAsync_ExistingWeapon_ReturnsNoContent()
        {
            // Arrange
            _fixture.MockCharacterService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Character);

            _fixture.MockWeaponService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Weapon);

            // Act
            var result = await _fixture.MockCharactersController.AddWeaponAsync(_fixture.ItemViewModel);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task RemoveWeaponAsync_ExistingWeapon_ReturnsNoContent()
        {
            // Arrange
            _fixture.MockCharacterService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Character);

            _fixture.MockWeaponService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Weapon);

            // Act
            var result = await _fixture.MockCharactersController.RemoveWeaponAsync(_fixture.ItemViewModel);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task AddSpellAsync_ExistingSpell_ReturnsNoContent()
        {
            // Arrange
            _fixture.MockCharacterService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Character);

            _fixture.MockSpellService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Spell);

            // Act
            var result = await _fixture.MockCharactersController.AddSpellAsync(_fixture.ItemViewModel);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task RemoveSpellAsync_ExistingSpell_ReturnsNoContent()
        {
            // Arrange
            _fixture.MockCharacterService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Character);

            _fixture.MockSpellService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Spell);

            // Act
            var result = await _fixture.MockCharactersController.RemoveSpellAsync(_fixture.ItemViewModel);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task AddMountAsync_ExistingMount_ReturnsNoContent()
        {
            // Arrange
            _fixture.MockCharacterService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Character);

            _fixture.MockMountService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Mount);

            // Act
            var result = await _fixture.MockCharactersController.AddMountAsync(_fixture.ItemViewModel);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task RemoveMountAsync_ExistingMount_ReturnsNoContent()
        {
            // Arrange
            _fixture.MockCharacterService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Character);

            _fixture.MockMountService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Mount);

            // Act
            var result = await _fixture.MockCharactersController.RemoveMountAsync(_fixture.ItemViewModel);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NoContentResult>(result);
        }
    }
}
