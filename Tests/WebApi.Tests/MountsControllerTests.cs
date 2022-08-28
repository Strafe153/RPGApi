using Core.Entities;
using Core.Models;
using Core.ViewModels;
using Core.ViewModels.MountViewModels;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Tests.Fixtures;
using Xunit;

namespace WebApi.Tests
{
    public class MountsControllerTests : IClassFixture<MountsControllerFixture>
    {
        private readonly MountsControllerFixture _fixture;

        public MountsControllerTests(MountsControllerFixture fixture)
        {
            _fixture = fixture;

            _fixture.MockControllerBaseUser();
            _fixture.MockObjectModelValidator(_fixture.MockMountsController);
        }

        [Fact]
        public async Task GetAsync_ValidPageParameters_ReturnsActionResultOfPageViewModelOfMountReadViewModel()
        {
            // Arrange
            _fixture.MockMountService
                .Setup(s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(_fixture.PagedList);

            _fixture.MockPaginatedMapper
                .Setup(m => m.Map(It.IsAny<PaginatedList<Mount>>()))
                .Returns(_fixture.PageViewModel);

            // Act
            var result = await _fixture.MockMountsController.GetAsync(_fixture.PageParameters);
            var pageViewModel = result.Result.As<OkObjectResult>().Value.As<PageViewModel<MountReadViewModel>>();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<PageViewModel<MountReadViewModel>>>();
            pageViewModel.Entities.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetAsync_ExistingMount_ReturnsActionResultOfMountReadViewModel()
        {
            // Arrange
            _fixture.MockMountService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Mount);

            _fixture.MockReadMapper
                .Setup(m => m.Map(It.IsAny<Mount>()))
                .Returns(_fixture.MountReadViewModel);

            // Act
            var result = await _fixture.MockMountsController.GetAsync(_fixture.Id);
            var readViewModel = result.Result.As<OkObjectResult>().Value.As<MountReadViewModel>();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<MountReadViewModel>>();
            readViewModel.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateAsync_ValidViewModel_ReturnsActionResultOfMountReadViewModel()
        {
            // Arrange
            _fixture.MockCreateMapper
                .Setup(m => m.Map(It.IsAny<MountBaseViewModel>()))
                .Returns(_fixture.Mount);

            // Act
            var result = await _fixture.MockMountsController.CreateAsync(_fixture.MountBaseViewModel);
            var readViewModel = result.Result.As<CreatedAtActionResult>().Value.As<MountReadViewModel>();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<MountReadViewModel>>();
            readViewModel.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateAsync_ExistingMountValidViewModel_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockMountService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Mount);

            // Act
            var result = await _fixture.MockMountsController.UpdateAsync(_fixture.Id, _fixture.MountBaseViewModel);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UpdateAsync_ExistingMountValidPatchDocument_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockMountService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Mount);

            _fixture.MockUpdateMapper
                .Setup(m => m.Map(_fixture.Mount))
                .Returns(_fixture.MountBaseViewModel);

            // Act
            var result = await _fixture.MockMountsController.UpdateAsync(_fixture.Id, _fixture.PatchDocument);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UpdateAsync_ExistingMountInvalidPatchDocument_ReturnsObjectResult()
        {
            // Arrange
            _fixture.MockMountService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Mount);

            _fixture.MockUpdateMapper
                .Setup(m => m.Map(_fixture.Mount))
                .Returns(_fixture.MountBaseViewModel);

            _fixture.MockModelError(_fixture.MockMountsController);

            // Act
            var result = await _fixture.MockMountsController.UpdateAsync(_fixture.Id, _fixture.PatchDocument);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ObjectResult>();
        }

        [Fact]
        public async Task DeleteAsync_ExistingMount_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockMountService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Mount);

            // Act
            var result = await _fixture.MockMountsController.DeleteAsync(_fixture.Id);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NoContentResult>();
        }
    }
}
