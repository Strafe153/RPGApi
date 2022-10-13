using Core.Dtos;
using Core.Dtos.MountDtos;
using Core.Entities;
using Core.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using WebApi.Tests.Fixtures;

namespace WebApi.Tests;

[TestFixture]
public class MountsControllerTests
{
    private MountsControllerFixture _fixture = default!;

    [OneTimeSetUp]
    public void SetUp()
    {
        _fixture = new MountsControllerFixture();
        _fixture.MockObjectModelValidator(_fixture.MountsController);
    }

    [TearDown]
    public void TearDown()
    {
        _fixture.MountsController.ControllerContext = _fixture.MockControllerContext();
    }

    [Test]
    public async Task GetAsync_ValidPageParameters_ReturnsActionResultOfPageDtoOfMountReadDto()
    {
        // Arrange
        _fixture.MountService
            .GetAllAsync(Arg.Any<int>(), Arg.Any<int>())
            .Returns(_fixture.PaginatedList);

        _fixture.PaginatedMapper
            .Map(Arg.Any<PaginatedList<Mount>>())
            .Returns(_fixture.PageDto);

        // Act
        var result = await _fixture.MountsController.GetAsync(_fixture.PageParameters, _fixture.CancellationToken);
        var objectResult = result.Result.As<OkObjectResult>();
        var pageDto = objectResult.Value.As<PageDto<MountReadDto>>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<ActionResult<PageDto<MountReadDto>>>();
        objectResult.StatusCode.Should().Be(200);
        pageDto.Entities.Should().NotBeEmpty();
    }

    [Test]
    public async Task GetAsync_ExistingMount_ReturnsActionResultOfMountReadDto()
    {
        // Arrange
        _fixture.MountService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Mount);

        _fixture.ReadMapper
            .Map(Arg.Any<Mount>())
            .Returns(_fixture.MountReadDto);

        // Act
        var result = await _fixture.MountsController.GetAsync(_fixture.Id, _fixture.CancellationToken);
        var objectResult = result.Result.As<OkObjectResult>();
        var readDto = objectResult.Value.As<MountReadDto>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<ActionResult<MountReadDto>>();
        objectResult.StatusCode.Should().Be(200);
        readDto.Should().NotBeNull();
    }

    [Test]
    public async Task CreateAsync_ValidDto_ReturnsActionResultOfMountReadDto()
    {
        // Arrange
        _fixture.CreateMapper
            .Map(Arg.Any<MountBaseDto>())
            .Returns(_fixture.Mount);

        _fixture.ReadMapper
            .Map(Arg.Any<Mount>())
            .Returns(_fixture.MountReadDto);

        // Act
        var result = await _fixture.MountsController.CreateAsync(_fixture.MountBaseDto);
        var objectResult = result.Result.As<CreatedAtActionResult>();
        var readDto = objectResult.Value.As<MountReadDto>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<ActionResult<MountReadDto>>();
        objectResult.StatusCode.Should().Be(201);
        readDto.Should().NotBeNull();
    }

    [Test]
    public async Task UpdateAsync_ExistingMountValidDto_ReturnsNoContentResult()
    {
        // Arrange
        _fixture.MountService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Mount);

        // Act
        var result = await _fixture.MountsController.UpdateAsync(_fixture.Id, _fixture.MountBaseDto);
        var objectResult = result.As<NoContentResult>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        objectResult.StatusCode.Should().Be(204);
    }

    [Test]
    public async Task UpdateAsync_ExistingMountValidPatchDocument_ReturnsNoContentResult()
    {
        // Arrange
        _fixture.MountService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Mount);

        _fixture.UpdateMapper
            .Map(Arg.Any<Mount>())
            .Returns(_fixture.MountBaseDto);

        // Act
        var result = await _fixture.MountsController.UpdateAsync(_fixture.Id, _fixture.PatchDocument);
        var objectResult = result.As<NoContentResult>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        objectResult.StatusCode.Should().Be(204);
    }

    [Test]
    public async Task UpdateAsync_ExistingMountInvalidPatchDocument_ReturnsObjectResult()
    {
        // Arrange
        _fixture.MountService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Mount);

        _fixture.UpdateMapper
            .Map(Arg.Any<Mount>())
            .Returns(_fixture.MountBaseDto);

        _fixture.MockModelError(_fixture.MountsController);

        // Act
        var result = await _fixture.MountsController.UpdateAsync(_fixture.Id, _fixture.PatchDocument);

        // Assert
        result.Should().NotBeNull().And.BeOfType<ObjectResult>();
    }

    [Test]
    public async Task DeleteAsync_ExistingMount_ReturnsNoContentResult()
    {
        // Arrange
        _fixture.MountService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Mount);

        // Act
        var result = await _fixture.MountsController.DeleteAsync(_fixture.Id);
        var objectResult = result.As<NoContentResult>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        objectResult.StatusCode.Should().Be(204);
    }
}
