using Core.Entities;
using Core.Models;
using Core.ViewModels;
using Core.ViewModels.SpellViewModels;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Tests.Fixtures;
using Xunit;

namespace WebApi.Tests
{
    public class SpellsControllerTests : IClassFixture<SpellsControllerFixture>
    {
        private readonly SpellsControllerFixture _fixture;

        public SpellsControllerTests(SpellsControllerFixture fixture)
        {
            _fixture = fixture;

            _fixture.MockObjectModelValidator(_fixture.MockSpellsController);
        }

        [Fact]
        public async Task GetAsync_ValidPageParameters_ReturnsActionResultOfPageViewModelOfSpellReadViewModel()
        {
            // Arrange
            _fixture.MockSpellService
                .Setup(s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(_fixture.PagedList);

            _fixture.MockPaginatedMapper
                .Setup(m => m.Map(It.IsAny<PaginatedList<Spell>>()))
                .Returns(_fixture.PageViewModel);

            // Act
            var result = await _fixture.MockSpellsController.GetAsync(_fixture.PageParameters);
            var pageViewModel = result.Result.As<OkObjectResult>().Value.As<PageViewModel<SpellReadViewModel>>();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<PageViewModel<SpellReadViewModel>>>();
            pageViewModel.Entities.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetAsync_ExistingSpell_ReturnsActionResultOfSpellReadViewModel()
        {
            // Arrange
            _fixture.MockSpellService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Spell);

            _fixture.MockReadMapper
                .Setup(m => m.Map(It.IsAny<Spell>()))
                .Returns(_fixture.SpellReadViewModel);

            // Act
            var result = await _fixture.MockSpellsController.GetAsync(_fixture.Id);
            var readViewModel = result.Result.As<OkObjectResult>().Value.As<SpellReadViewModel>();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<SpellReadViewModel>>();
            readViewModel.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateAsync_ValidViewModel_ReturnsActionResultOfSpellReadViewModel()
        {
            // Arrange
            _fixture.MockCreateMapper
                .Setup(m => m.Map(It.IsAny<SpellBaseViewModel>()))
                .Returns(_fixture.Spell);

            // Act
            var result = await _fixture.MockSpellsController.CreateAsync(_fixture.SpellBaseViewModel);
            var readViewModel = result.Result.As<CreatedAtActionResult>().Value.As<SpellReadViewModel>();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<SpellReadViewModel>>();
            readViewModel.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateAsync_ExistingSpellValidViewModel_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockSpellService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Spell);

            // Act
            var result = await _fixture.MockSpellsController.UpdateAsync(_fixture.Id, _fixture.SpellBaseViewModel);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UpdateAsync_ExistingSpellValidPatchDocument_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockSpellService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Spell);

            _fixture.MockUpdateMapper
                .Setup(m => m.Map(_fixture.Spell))
                .Returns(_fixture.SpellBaseViewModel);

            // Act
            var result = await _fixture.MockSpellsController.UpdateAsync(_fixture.Id, _fixture.PatchDocument);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UpdateAsync_InvalidPatchDocument_ReturnsObjectResult()
        {
            // Arrange
            _fixture.MockSpellService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Spell);

            _fixture.MockUpdateMapper
                .Setup(m => m.Map(_fixture.Spell))
                .Returns(_fixture.SpellBaseViewModel);

            _fixture.MockModelError(_fixture.MockSpellsController);

            // Act
            var result = await _fixture.MockSpellsController.UpdateAsync(_fixture.Id, _fixture.PatchDocument);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ObjectResult>();
        }

        [Fact]
        public async Task DeleteAsync_ExistingSpell_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockSpellService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Spell);

            // Act
            var result = await _fixture.MockSpellsController.DeleteAsync(_fixture.Id);

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
                .Setup(s => s.GetSpell(It.IsAny<Character>(), It.IsAny<int>()))
                .Returns(_fixture.Spell);

            // Act
            var result = await _fixture.MockSpellsController.HitAsync(_fixture.HitViewModel);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NoContentResult>();
        }
    }
}
