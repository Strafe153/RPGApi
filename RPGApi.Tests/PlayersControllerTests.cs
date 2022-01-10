namespace RPGApi.Tests
{
    public class PlayersControllerTests
    {
        private static readonly Mock<IControllerRepository<Player>> _repo = new();
        private static readonly Mock<IMapper> _mapper = new();
        private static readonly PlayersController _controller = new(_repo.Object, _mapper.Object);

        [Fact]
        public async Task GetPlayersAsync_ExistingItems_ReturnsActionResultOfReadDtos()
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
            var result = await _controller.CreatePlayerAsync(new PlayerCreateUpdateDto());

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
            var result = await _controller.UpdatePlayerAsync(Guid.Empty, new PlayerCreateUpdateDto());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdatePlayerAsync_NonexistingPlayer_ReturnsNotFoundResult()
        {
            // Arrange
            _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Player?)null);

            // Act
            var result = await _controller.UpdatePlayerAsync(Guid.Empty, new PlayerCreateUpdateDto());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task PartialUpdatePlayerAsync_ExistingPlayer_ReturnsNoContentResult()
        {
            // Arrange
            _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Player());
            _mapper.Setup(m => m.Map<PlayerCreateUpdateDto>(It.IsAny<Player>()))
                .Returns(new PlayerCreateUpdateDto());

            Utility.MockObjectModelValidator(_controller);

            // Act
            var result = await _controller.PartialUpdatePlayerAsync(Guid.Empty, 
                new JsonPatchDocument<PlayerCreateUpdateDto>());

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
                new JsonPatchDocument<PlayerCreateUpdateDto>());

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