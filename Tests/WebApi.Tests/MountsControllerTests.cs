using Core.Dtos;
using Core.Dtos.MountDtos;
using Core.Entities;
using Core.Models;
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
        public async Task GetAsync_ValidPageParameters_ReturnsActionResultOfPageDtoOfMountReadDto()
        {
            // Arrange
            _fixture.MockMountService
                .Setup(s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(_fixture.PaginatedList);

            _fixture.MockPaginatedMapper
                .Setup(m => m.Map(It.IsAny<PaginatedList<Mount>>()))
                .Returns(_fixture.PageDto);

            // Act
            var result = await _fixture.MockMountsController.GetAsync(_fixture.PageParameters);
            var pageDto = result.Result.As<OkObjectResult>().Value.As<PageDto<MountReadDto>>();

            // Assert
            result.Should().NotBeNull().And.BeOfType<ActionResult<PageDto<MountReadDto>>>();
            pageDto.Entities.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetAsync_ExistingMount_ReturnsActionResultOfMountReadDto()
        {
            // Arrange
            _fixture.MockMountService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Mount);

            _fixture.MockReadMapper
                .Setup(m => m.Map(It.IsAny<Mount>()))
                .Returns(_fixture.MountReadDto);

            // Act
            var result = await _fixture.MockMountsController.GetAsync(_fixture.Id);
            var readDto = result.Result.As<OkObjectResult>().Value.As<MountReadDto>();

            // Assert
            result.Should().NotBeNull().And.BeOfType<ActionResult<MountReadDto>>();
            readDto.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateAsync_ValidDto_ReturnsActionResultOfMountReadDto()
        {
            // Arrange
            _fixture.MockCreateMapper
                .Setup(m => m.Map(It.IsAny<MountBaseDto>()))
                .Returns(_fixture.Mount);

            // Act
            var result = await _fixture.MockMountsController.CreateAsync(_fixture.MountBaseDto);
            var readDto = result.Result.As<CreatedAtActionResult>().Value.As<MountReadDto>();

            // Assert
            result.Should().NotBeNull().And.BeOfType<ActionResult<MountReadDto>>();
            readDto.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateAsync_ExistingMountValidDto_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockMountService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Mount);

            // Act
            var result = await _fixture.MockMountsController.UpdateAsync(_fixture.Id, _fixture.MountBaseDto);

            // Assert
            result.Should().NotBeNull().And.BeOfType<NoContentResult>();
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
                .Returns(_fixture.MountBaseDto);

            // Act
            var result = await _fixture.MockMountsController.UpdateAsync(_fixture.Id, _fixture.PatchDocument);

            // Assert
            result.Should().NotBeNull().And.BeOfType<NoContentResult>();
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
                .Returns(_fixture.MountBaseDto);

            _fixture.MockModelError(_fixture.MockMountsController);

            // Act
            var result = await _fixture.MockMountsController.UpdateAsync(_fixture.Id, _fixture.PatchDocument);

            // Assert
            result.Should().NotBeNull().And.BeOfType<ObjectResult>();
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
            result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        }
    }
}
