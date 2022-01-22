namespace RPGApi.Tests
{
    public class PlayersControllerTests
    {
        private static readonly Mock<IPlayerControllerRepository> _repo = new();
        private static readonly Mock<IMapper> _mapper = new();
        private static readonly PlayersController _controller = new(_repo.Object, _mapper.Object);

        private static readonly Player _player = new();

        [Fact]
        public async Task GetAllPlayersAsync_Items_ReturnsActionResultOfReadDtos()
        {
            // Arrange
            _repo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Player>());

            // Act
            var result = await _controller.GetAllPlayersAsync();

            // Assert
            Assert.IsType<ActionResult<IEnumerable<PlayerReadDto>>>(result);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPaginatedPlayersAsync_Items_ReturnsActionResultOfPageDto()
        {
            // Arrange
            _repo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Player>());

            // Act
            var result = await _controller.GetPaginatedPlayersAsync(It.IsAny<int>());

            // Assert
            Assert.IsType<ActionResult<PageDto<PlayerReadDto>>>(result);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPlayerAsync_ExistingPlayer_ReturnsActionResultOfReadDto()
        {
            // Arrange
            _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(_player);

            // Act
            var result = await _controller.GetPlayerAsync(Guid.Empty);

            // Assert
            Assert.IsType<ActionResult<PlayerReadDto>>(result);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPlayerAsync_NonexistingPlayer_ReturnsNotFoundResult()
        {
            // Arrange
            _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Player?)null);

            // Act
            var result = await _controller.GetPlayerAsync(Guid.Empty);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task RegisterPlayerAsync_NonexistingPlayer_ReturnsActionResultOfReadDto()
        {
            // Arrange
            _repo.Setup(r => r.GetByNameAsync(It.IsAny<string>())).ReturnsAsync((Player?)null);
            _mapper.Setup(m => m.Map<PlayerReadDto>(It.IsAny<Player>()))
                .Returns(new PlayerReadDto());

            // Act
            var result = await _controller.RegisterPlayerAsync(new PlayerAuthorizeDto());

            // Assert
            Assert.IsType<ActionResult<PlayerReadDto>>(result);
            Assert.IsType<CreatedAtActionResult>(result.Result);
        }

        [Fact]
        public async Task RegisterPlayerAsync_ExistingPlayer_ReturnsBadRequestObjectResult()
        {
            // Arrange
            _repo.Setup(r => r.GetByNameAsync(It.IsAny<string>())).ReturnsAsync(_player);

            // Act
            var result = await _controller.RegisterPlayerAsync(new PlayerAuthorizeDto());

            // Assert
            Assert.IsType<ActionResult<PlayerReadDto>>(result);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task LoginPlayerAsync_ExistingPlayer_ReturnsActionResultOfReadDto()
        {
            // Arrange
            _repo.Setup(r => r.GetByNameAsync(It.IsAny<string>())).ReturnsAsync(_player);

            // Act
            var result = await _controller.LoginPlayerAsync(new PlayerAuthorizeDto());

            // Assert
            Assert.IsType<ActionResult<PlayerWithTokenReadDto>>(result);
        }

        [Fact]
        public async Task LoginPlayerAsync_NonexistingPlayer_ReturnsNotFoundResult()
        {
            // Arrange
            _repo.Setup(r => r.GetByNameAsync(It.IsAny<string>())).ReturnsAsync((Player?)null);

            // Act
            var result = await _controller.LoginPlayerAsync(new PlayerAuthorizeDto());

            // Assert
            Assert.IsType<ActionResult<PlayerWithTokenReadDto>>(result);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task LoginPlayerAsync_DifferentNames_ReturnsBadRequestObjectResult()
        {
            // Arrange
            _player.Name = "player_name";
            _repo.Setup(r => r.GetByNameAsync(It.IsAny<string>())).ReturnsAsync(_player);

            // Act
            var result = await _controller.LoginPlayerAsync(
                new PlayerAuthorizeDto() { Name = "dto_name" });

            // Assert
            Assert.IsType<ActionResult<PlayerWithTokenReadDto>>(result);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task LoginPlayerAsync_NotVerifiedPasswordHash_ReturnsBadRequestObjectResult()
        {
            // Arrange
            _repo.Setup(r => r.GetByNameAsync(It.IsAny<string>())).ReturnsAsync(new Player());
            _repo.Setup(r => r.VerifyPasswordHash(It.IsAny<string>(), It.IsAny<byte[]>(),
                It.IsAny<byte[]>())).Returns(false);

            // Act
            var result = await _controller.LoginPlayerAsync(new PlayerAuthorizeDto());

            // Assert
            Assert.IsType<ActionResult<PlayerWithTokenReadDto>>(result);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdatePlayerAsync_ExistingPlayer_ReturnsNoContentResult()
        {
            // Arrange
            _player.Name = "identity_name";
            _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(_player); ;

            // Act
            var result = await _controller.UpdatePlayerAsync(Guid.Empty, new PlayerUpdateDto());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdatePlayerAsync_NonexistingPlayer_ReturnsNotFoundResult()
        {
            // Arrange
            _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Player?)null);

            // Act
            var result = await _controller.UpdatePlayerAsync(Guid.Empty, new PlayerUpdateDto());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdatePlayerAsync_NoAccessRights_ReturnsForbidResult()
        {
            // Arrange
            _player.Name = "player_name";
            _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(_player);

            Utility.MockUserIdentityName(_controller);

            // Act
            var result = await _controller.UpdatePlayerAsync(Guid.Empty, new PlayerUpdateDto());

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task PartialUpdatePlayerAsync_ExistingPlayer_ReturnsNoContentResult()
        {
            // Arrange
            _player.Name = "identity_name";
            _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(_player);
            _mapper.Setup(m => m.Map<PlayerUpdateDto>(It.IsAny<Player>()))
                .Returns(new PlayerUpdateDto());

            Utility.MockObjectModelValidator(_controller);

            // Act
            var result = await _controller.PartialUpdatePlayerAsync(Guid.Empty, 
                new JsonPatchDocument<PlayerUpdateDto>());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PartialUpdatePlayerAsync_NonexistingPlayer_ReturnsNotFoundResult()
        {
            // Arrange
            _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Player?)null);

            // Act
            var result = await _controller.PartialUpdatePlayerAsync(Guid.Empty,
                new JsonPatchDocument<PlayerUpdateDto>());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task PartialUpdatePlayerAsync_NoAccessRights_ReturnsForbidResult()
        {
            // Arrange
            _player.Name = "player_name";
            _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(_player);
            _mapper.Setup(m => m.Map<PlayerUpdateDto>(It.IsAny<Player>()))
                .Returns(new PlayerUpdateDto());

            Utility.MockObjectModelValidator(_controller);
            Utility.MockUserIdentityName(_controller);

            // Act
            var result = await _controller.PartialUpdatePlayerAsync(Guid.Empty,
                new JsonPatchDocument<PlayerUpdateDto>());

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task DeletePlayerAsync_ExistingPlayer_ReturnsNoContentResult()
        {
            // Arrange
            _player.Name = "identity_name";
            _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(_player);

            // Act
            var result = await _controller.DeletePlayerAsync(Guid.Empty);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeletePlayerAsync_NonexistingPlayer_ReturnsNotFoundResult()
        {
            // Arrange
            _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Player?)null);

            // Act
            var result = await _controller.DeletePlayerAsync(Guid.Empty);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeletePlayerAsync_NoAccessRights_ReturnsForbidResult()
        {
            // Arrange
            _player.Name = "player_name";
            _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(_player);

            Utility.MockUserIdentityName(_controller);

            // Act
            var result = await _controller.DeletePlayerAsync(Guid.Empty);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task ChangeRoleAsync_ExistingPlayer_ReturnsNoContentResult()
        {
            // Arrange
            _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(_player);

            // Act
            var result = await _controller.ChangeRoleAsync(new PlayerChangeRoleDto());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task ChangeRoleAsync_NonexistingPlayer_ReturnsNotFoundResult()
        {
            // Arrange
            _player.Name = "player_name";
            _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Player?)null);

            // Act
            var result = await _controller.ChangeRoleAsync(new PlayerChangeRoleDto());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}