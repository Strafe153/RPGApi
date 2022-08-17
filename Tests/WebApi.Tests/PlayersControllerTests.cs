using Core.Entities;
using Core.Models;
using Core.ViewModels;
using Core.ViewModels.PlayerViewModels;
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
        public async Task GetAsync_ValidPageParameters_ReturnsOkObjectResult()
        {
            // Arrange
            _fixture.MockPlayerService
                .Setup(s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(_fixture.PagedList);

            _fixture.MockPagedMapper
                .Setup(m => m.Map(It.IsAny<PagedList<Player>>()))
                .Returns(_fixture.PageViewModel);

            // Act
            var result = await _fixture.MockPlayersController.GetAsync(_fixture.PageParameters);
            var pageViewModels = (result.Result as OkObjectResult)!.Value as PageViewModel<PlayerReadViewModel>;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ActionResult<PageViewModel<PlayerReadViewModel>>>(result);
            Assert.NotNull(pageViewModels!.Entities);
        }

        [Fact]
        public async Task GetAsync_ValidId_ReturnsOkObjectResult()
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
            var readViewModel = (result.Result as OkObjectResult)!.Value as PlayerReadViewModel;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ActionResult<PlayerReadViewModel>>(result);
            Assert.NotNull(readViewModel);
        }

        [Fact]
        public async Task RegisterAsync_ValidViewModel_ReturnsCreatedAtActionResult()
        {
            // Arrange
            _fixture.MockReadMapper
                .Setup(m => m.Map(It.IsAny<Player>()))
                .Returns(_fixture.PlayerReadViewModel);

            // Act
            var result = await _fixture.MockPlayersController.RegisterAsync(_fixture.PlayerAuthorizeViewModel);
            var readViewModel = (result.Result as CreatedAtActionResult)!.Value as PlayerReadViewModel;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ActionResult<PlayerReadViewModel>>(result);
            Assert.NotNull(readViewModel);
        }

        [Fact]
        public async Task LoginAsync_ValidViewModel_ReturnsOkObjectResult()
        {
            // Arrange
            _fixture.MockReadWithTokenMapper
                .Setup(m => m.Map(It.IsAny<Player>()))
                .Returns(_fixture.PlayerWithTokenReadViewModel);

            // Act
            var result = await _fixture.MockPlayersController.LoginAsync(_fixture.PlayerAuthorizeViewModel);
            var readWithTokenViewModel = (result.Result as OkObjectResult)!.Value as PlayerWithTokenReadViewModel;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ActionResult<PlayerWithTokenReadViewModel>>(result);
            Assert.NotNull(readWithTokenViewModel);
        }

        [Fact]
        public async Task UpdateAsync_ValidName_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockPlayerService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Player);

            // Act
            var result = await _fixture.MockPlayersController.UpdateAsync(_fixture.Id, _fixture.PlayerUpdateViewModel);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task ChangePasswordAsync_ValidPassword_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockPlayerService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Player);

            // Act
            var result = await _fixture.MockPlayersController
                .ChangePasswordAsync(_fixture.Id, _fixture.PlayerUpdateViewModel!);
            var readToken = result.Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ActionResult<string>>(result);
            Assert.NotNull(readToken);
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
            Assert.NotNull(result);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task ChangeRoleAsync_ExistingPlayer_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockPlayerService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Player);

            // Act
            var result = await _fixture.MockPlayersController
                .ChangeRoleAsync(_fixture.Id, _fixture.PlayerChangeRoleViewModel);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NoContentResult>(result);
        }
    }
}
