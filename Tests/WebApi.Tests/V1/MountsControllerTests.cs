using Domain.Dtos;
using Domain.Dtos.MountDtos;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using WebApi.Tests.V1.Fixtures;

namespace WebApi.Tests.V1;

[TestFixture]
public class MountsControllerTests
{
    private MountsControllerFixture _fixture = default!;

    [SetUp]
    public void SetUp()
    {
        _fixture = new MountsControllerFixture();
        _fixture.MockObjectModelValidator(_fixture.MountsController);
    }

    [TearDown]
    public void TearDown() => _fixture.MountsController.ControllerContext = _fixture.MockControllerContext();

    [Test]
    public async Task Get_Should_ReturnActionResultOfPageDtoOfMountReadDto_WhenPageParametersAreValid()
    {
        // Arrange
        _fixture.MountService
            .GetAllAsync(Arg.Any<int>(), Arg.Any<int>())
            .Returns(_fixture.PaginatedList);

        // Act
        var result = await _fixture.MountsController.Get(_fixture.PageParameters, _fixture.CancellationToken);
        var objectResult = result.Result.As<OkObjectResult>();
        var pageDto = objectResult.Value.As<PageDto<MountReadDto>>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<ActionResult<PageDto<MountReadDto>>>();
        objectResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        pageDto.Entities!.Count().Should().Be(_fixture.MountsCount);
    }

    [Test]
    public async Task Get_Should_ReturnActionREsultOfMountReadDto_WhenMountExists()
    {
        // Arrange
        _fixture.MountService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Mount);

        // Act
        var result = await _fixture.MountsController.Get(_fixture.Id, _fixture.CancellationToken);
        var objectResult = result.Result.As<OkObjectResult>();
        var readDto = objectResult.Value.As<MountReadDto>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<ActionResult<MountReadDto>>();
        objectResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        readDto.Should().NotBeNull();
    }

    [Test]
    public async Task Create_Should_ReturnActionResultOfMountReadDto_WhenMountCreateDtoIsValid()
    {
        // Act
        var result = await _fixture.MountsController.Create(_fixture.MountBaseDto);
        var objectResult = result.Result.As<CreatedAtActionResult>();
        var readDto = objectResult.Value.As<MountReadDto>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<ActionResult<MountReadDto>>();
        objectResult.StatusCode.Should().Be(StatusCodes.Status201Created);
        readDto.Should().NotBeNull();
    }

    [Test]
    public async Task Update_Should_ReturnNoContentResult_WhenMountExistsAndMountUpdateDtoIsValid()
    {
        // Arrange
        _fixture.MountService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Mount);

        // Act
        var result = await _fixture.MountsController.Update(_fixture.Id, _fixture.MountBaseDto);
        var objectResult = result.As<NoContentResult>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        objectResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }

    [Test]
    public async Task Update_Should_ReturnNoContentResult_WhenMountExistsAndPatchDocumentIsValid()
    {
        // Arrange
        _fixture.MountService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Mount);

        // Act
        var result = await _fixture.MountsController.Update(_fixture.Id, _fixture.PatchDocument);
        var objectResult = result.As<NoContentResult>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        objectResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }

    [Test]
    public async Task Update_Should_ReturnObjectResult_WhenMountExistsAndPatchDocumentIsInvalid()
    {
        // Arrange
        _fixture.MountService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Mount);

        _fixture.MockModelError(_fixture.MountsController);

        // Act
        var result = await _fixture.MountsController.Update(_fixture.Id, _fixture.PatchDocument);

        // Assert
        result.Should().NotBeNull().And.BeOfType<ObjectResult>();
    }

    [Test]
    public async Task Delete_Should_ReturnNoContentResult_WhenMountExists()
    {
        // Arrange
        _fixture.MountService
            .GetByIdAsync(Arg.Any<int>())
            .Returns(_fixture.Mount);

        // Act
        var result = await _fixture.MountsController.Delete(_fixture.Id);
        var objectResult = result.As<NoContentResult>();

        // Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        objectResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }
}
