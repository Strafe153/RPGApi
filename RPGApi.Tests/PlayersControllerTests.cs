namespace RPGApi.Tests
{
    public class PlayersControllerTests
    {
        private static readonly Mock<IPlayerControllerRepository> _repo = new();
        private static readonly Mock<IMapper> _mapper = new();
        private static readonly PlayersController _controller = new(_repo.Object, _mapper.Object);

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
            _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Player());

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
        public async Task CreatePlayerAsync_ValidData_ReturnsActionResultOfReadDto()
        {
            // Arrange
            _mapper.Setup(m => m.Map<PlayerReadDto>(It.IsAny<Player>()))
                .Returns(new PlayerReadDto());

            // Act
            var result = await _controller.RegisterPlayerAsync(new PlayerAuthorizeDto());

            // Assert
            Assert.IsType<ActionResult<PlayerReadDto>>(result);
            Assert.IsType<CreatedAtActionResult>(result.Result);
        }

        [Fact]
        public async Task UpdatePlayerAsync_ExistingPlayer_ReturnsNoContentResult()
        {
            // Arrange
            _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Player());

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
        public async Task PartialUpdatePlayerAsync_ExistingPlayer_ReturnsNoContentResult()
        {
            // Arrange
            _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Player());
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
        public async Task DeletePlayerAsync_ExistingPlayer_ReturnsNoContentResult()
        {
            // Arrange
            _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Player());

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
    }
}