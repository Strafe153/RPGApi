using Core.Dtos;
using Core.Dtos.PlayerDtos;
using Core.Entities;
using Core.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Tests.Fixtures;
using Xunit;

namespace WebApi.Tests
{
    public class PlayersControllerTests : IClassFixture<PlayersControllerFixture>
    {
        private readonly PlayersControllerFixture _fixture;

        public PlayersControllerTests(PlayersControllerFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetAsync_ValidPageParameters_ReturnsActionResultOfPageDtoOfPlayerReadDto()
        {
            // Arrange
            _fixture.MockPlayerService
                .Setup(s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(_fixture.PaginatedList);

            _fixture.MockPaginatedMapper
                .Setup(m => m.Map(It.IsAny<PaginatedList<Player>>()))
                .Returns(_fixture.PageDto);

            // Act
            var result = await _fixture.MockPlayersController.GetAsync(_fixture.PageParameters);
            var pageDto = result.Result.As<OkObjectResult>().Value.As<PageDto<PlayerReadDto>>();

            // Assert
            result.Should().NotBeNull().And.BeOfType<ActionResult<PageDto<PlayerReadDto>>>();
            pageDto.Entities.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetAsync_ExistingPlayer_ReturnsActionResultOfPlayerReadDto()
        {
            // Arrange
            _fixture.MockPlayerService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Player);

            _fixture.MockReadMapper
                .Setup(m => m.Map(It.IsAny<Player>()))
                .Returns(_fixture.PlayerReadDto);

            // Act
            var result = await _fixture.MockPlayersController.GetAsync(_fixture.Id);
            var readDto = result.Result.As<OkObjectResult>().Value.As<PlayerReadDto>();

            // Assert
            result.Should().NotBeNull().And.BeOfType<ActionResult<PlayerReadDto>>();
            readDto.Should().NotBeNull();
        }

        [Fact]
        public async Task RegisterAsync_ValidDto_ReturnsActionResultOfPlayerReadDto()
        {
            // Arrange
            _fixture.MockReadMapper
                .Setup(m => m.Map(It.IsAny<Player>()))
                .Returns(_fixture.PlayerReadDto);

            // Act
            var result = await _fixture.MockPlayersController.RegisterAsync(_fixture.PlayerAuthorizeDto);
            var readDto = result.Result.As<CreatedAtActionResult>().Value.As<PlayerReadDto>();

            // Assert
            result.Should().NotBeNull().And.BeOfType<ActionResult<PlayerReadDto>>();
            readDto.Should().NotBeNull();
        }

        [Fact]
        public async Task LoginAsync_ValidDto_ReturnsActionResultOfPlayerWithTokenReadDto()
        {
            // Arrange
            _fixture.MockReadWithTokenMapper
                .Setup(m => m.Map(It.IsAny<Player>()))
                .Returns(_fixture.PlayerWithTokenReadDto);

            // Act
            var result = await _fixture.MockPlayersController.LoginAsync(_fixture.PlayerAuthorizeDto);
            var readWithTokenDto = result.Result.As<OkObjectResult>().Value.As<PlayerWithTokenReadDto>();

            // Assert
            result.Should().NotBeNull().And.BeOfType<ActionResult<PlayerWithTokenReadDto>>();
            readWithTokenDto.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateAsync_ValidDto_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockPlayerService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Player);

            // Act
            var result = await _fixture.MockPlayersController.UpdateAsync(_fixture.Id, _fixture.PlayerUpdateDto);

            // Assert
            result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task ChangePasswordAsync_ValidDto_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockPlayerService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Player);

            // Act
            var result = await _fixture.MockPlayersController.ChangePasswordAsync(_fixture.Id, _fixture.PlayerChangePasswordDto);

            // Assert
            result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteAsync_ExistingPlayer_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockPlayerService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Player);

            // Act
            var result = await _fixture.MockPlayersController.DeleteAsync(_fixture.Id);

            // Assert
            result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task ChangeRoleAsync_ExistingPlayerValidRole_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockPlayerService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Player);

            // Act
            var result = await _fixture.MockPlayersController.ChangeRoleAsync(_fixture.Id, _fixture.PlayerChangeRoleDto);

            // Assert
            result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        }
    }
}
