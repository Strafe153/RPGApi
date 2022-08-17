using Core.Entities;
using Core.Models;
using Core.ViewModels;
using Core.ViewModels.MountViewModels;
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
        public async Task GetAsync_ValidPageParameters_ReturnsOkObjectResult()
        {
            // Arrange
            _fixture.MockMountService
                .Setup(s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(_fixture.PagedList);

            _fixture.MockPagedMapper
                .Setup(m => m.Map(It.IsAny<PagedList<Mount>>()))
                .Returns(_fixture.PageViewModel);

            // Act
            var result = await _fixture.MockMountsController.GetAsync(_fixture.PageParameters);
            var pageViewModel = (result.Result as OkObjectResult)!.Value as PageViewModel<MountReadViewModel>;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ActionResult<PageViewModel<MountReadViewModel>>>(result);
            Assert.NotEmpty(pageViewModel!.Entities!);
        }

        [Fact]
        public async Task GetAsync_ValidId_ReturnsOkObjectResult()
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
            var readViewModel = (result.Result as OkObjectResult)!.Value as MountReadViewModel;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ActionResult<MountReadViewModel>>(result);
            Assert.NotNull(readViewModel);
        }

        [Fact]
        public async Task CreateAsync_ValidViewModel_ReturnsCreatedAtActionResult()
        {
            // Arrange
            _fixture.MockCreateMapper
                .Setup(m => m.Map(It.IsAny<MountBaseViewModel>()))
                .Returns(_fixture.Mount);

            // Act
            var result = await _fixture.MockMountsController.CreateAsync(_fixture.MountBaseViewModel);
            var readViewModel = (result.Result as CreatedAtActionResult)!.Value as MountReadViewModel;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ActionResult<MountReadViewModel>>(result);
            Assert.NotNull(readViewModel);
        }

        [Fact]
        public async Task UpdateAsync_ValidViewModel_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockMountService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Mount);

            // Act
            var result = await _fixture.MockMountsController.UpdateAsync(_fixture.Id, _fixture.MountBaseViewModel);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateAsync_ValidPatchDocument_ReturnsNoContentResult()
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
            Assert.NotNull(result);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateAsync_InvalidPatchDocument_ReturnsObjectResult()
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
            Assert.NotNull(result);
            Assert.IsType<ObjectResult>(result);
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
            Assert.NotNull(result);
            Assert.IsType<NoContentResult>(result);
        }
    }
}
