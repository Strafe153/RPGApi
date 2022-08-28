using Core.Entities;
using Core.Models;
using Core.ViewModels;
using Core.ViewModels.PlayerViewModels;
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
        public async Task GetAsync_ValidPageParameters_ReturnsActionResultOfPageViewModelOfPlayerReadViewModel()
        {
            // Arrange
            _fixture.MockPlayerService
                .Setup(s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(_fixture.PagedList);

            _fixture.MockPaginatedMapper
                .Setup(m => m.Map(It.IsAny<PaginatedList<Player>>()))
                .Returns(_fixture.PageViewModel);

            // Act
            var result = await _fixture.MockPlayersController.GetAsync(_fixture.PageParameters);
            var pageViewModel = result.Result.As<OkObjectResult>().Value.As<PageViewModel<PlayerReadViewModel>>();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<PageViewModel<PlayerReadViewModel>>>();
            pageViewModel.Entities.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetAsync_ExistingPlayer_ReturnsActionResultOfPlayerReadViewModel()
        {
            // Arrange
            _fixture.MockPlayerService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Player);

            _fixture.MockReadMapper
                .Setup(m => m.Map(It.IsAny<Player>()))
                .Returns(_fixture.PlayerReadViewModel);

            // Act
            var result = await _fixture.MockPlayersController.GetAsync(_fixture.Id);
            var readViewModel = result.Result.As<OkObjectResult>().Value.As<PlayerReadViewModel>();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<PlayerReadViewModel>>();
            readViewModel.Should().NotBeNull();
        }

        [Fact]
        public async Task RegisterAsync_ValidViewModel_ReturnsActionResultOfPlayerReadViewModel()
        {
            // Arrange
            _fixture.MockReadMapper
                .Setup(m => m.Map(It.IsAny<Player>()))
                .Returns(_fixture.PlayerReadViewModel);

            // Act
            var result = await _fixture.MockPlayersController.RegisterAsync(_fixture.PlayerAuthorizeViewModel);
            var readViewModel = result.Result.As<CreatedAtActionResult>().Value.As<PlayerReadViewModel>();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<PlayerReadViewModel>>();
            readViewModel.Should().NotBeNull();
        }

        [Fact]
        public async Task LoginAsync_ValidViewModel_ReturnsActionResultOfPlayerWithTokenReadViewModel()
        {
            // Arrange
            _fixture.MockReadWithTokenMapper
                .Setup(m => m.Map(It.IsAny<Player>()))
                .Returns(_fixture.PlayerWithTokenReadViewModel);

            // Act
            var result = await _fixture.MockPlayersController.LoginAsync(_fixture.PlayerAuthorizeViewModel);
            var readWithTokenViewModel = result.Result.As<OkObjectResult>().Value.As<PlayerWithTokenReadViewModel>();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<PlayerWithTokenReadViewModel>>();
            readWithTokenViewModel.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateAsync_ValidViewModel_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockPlayerService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Player);

            // Act
            var result = await _fixture.MockPlayersController.UpdateAsync(_fixture.Id, _fixture.PlayerUpdateViewModel);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task ChangePasswordAsync_ValidViewModel_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockPlayerService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Player);

            // Act
            var result = await _fixture.MockPlayersController
                .ChangePasswordAsync(_fixture.Id, _fixture.PlayerChangePasswordViewModel);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NoContentResult>();
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
            result.Should().NotBeNull();
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task ChangeRoleAsync_ExistingPlayerValidRole_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockPlayerService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Player);

            // Act
            var result = await _fixture.MockPlayersController.ChangeRoleAsync(_fixture.Id, _fixture.PlayerChangeRoleViewModel);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NoContentResult>();
        }
    }
}
