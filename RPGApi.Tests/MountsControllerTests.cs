namespace RPGApi.Tests
{
    public class MountsControllerTests
    {
        private static readonly Mock<IControllerRepository<Mount>> _repo = new();
        private static readonly Mock<IMapper> _mapper = new();
        private static readonly MountsController _controller = new(_repo.Object, _mapper.Object);

        [Fact]
        public async Task GetAllMountsAsync_Items_ReturnsActionResultOfReadDtos()
        {
            // Arrange
            _repo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Mount>());

            // Act
            var result = await _controller.GetAllMountsAsync();

            // Assert
            Assert.IsType<ActionResult<IEnumerable<MountReadDto>>>(result);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPaginatedMountsAsync_Items_ReturnsActionResultOfPageDto()
        {
            // Arrange
            _repo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Mount>());

            // Act
            var result = await _controller.GetPaginatedMountsAsync(It.IsAny<int>());

            // Assert
            Assert.IsType<ActionResult<PageDto<MountReadDto>>>(result);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetMountAsync_ExistingMount_ReturnsActionResultOfReadDto()
        {
            // Arrange
            _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Mount());

            // Act
            var result = await _controller.GetMountAsync(Guid.Empty);

            // Assert
            Assert.IsType<ActionResult<MountReadDto>>(result);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetMountAsync_NonexistingMount_ReturnsNotFoundResult()
        {
            // Arrange
            _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Mount?)null);

            // Act
            var result = await _controller.GetMountAsync(Guid.Empty);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateMountAsync_ValidData_ReturnsActionResultOfReadDto()
        {
            // Arrange
            _mapper.Setup(m => m.Map<MountReadDto>(It.IsAny<Mount>()))
                .Returns(new MountReadDto());

            // Act
            var result = await _controller.CreateMountAsync(new MountCreateUpdateDto());

            // Assert
            Assert.IsType<ActionResult<MountReadDto>>(result);
            Assert.IsType<CreatedAtActionResult>(result.Result);
        }

        [Fact]
        public async Task UpdateMountAsync_ExistingMount_ReturnsNoContentResult()
        {
            // Arrange
            _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Mount());

            // Act
            var result = await _controller.UpdateMountAsync(Guid.Empty, new MountCreateUpdateDto());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateMountAsync_NonexistingMount_ReturnsNotFoundResult()
        {
            // Arrange
            _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Mount?)null);

            // Act
            var result = await _controller.UpdateMountAsync(Guid.Empty, new MountCreateUpdateDto());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task PartialUpdateMountAsync_ExistingMount_ReturnsNoContentResult()
        {
            // Arrange
            _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Mount());
            _mapper.Setup(m => m.Map<MountCreateUpdateDto>(It.IsAny<Mount>()))
                .Returns(new MountCreateUpdateDto());

            Utility.MockObjectModelValidator(_controller);

            // Act
            var result = await _controller.PartialUpdateMountAsync(Guid.Empty,
                new JsonPatchDocument<MountCreateUpdateDto>());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PartialUpdateMountAsync_NonexistingMount_ReturnsNotFoundResult()
        {
            // Arrange
            _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Mount?)null);

            // Act
            var result = await _controller.PartialUpdateMountAsync(Guid.Empty,
                new JsonPatchDocument<MountCreateUpdateDto>());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteMountAsync_ExistingMount_ReturnsNoContentResult()
        {
            // Arrange
            _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Mount());

            // Act
            var result = await _controller.DeleteMountAsync(Guid.Empty);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteMountAsync_NonexistingMount_ReturnsNotFoundResult()
        {
            // Arrange
            _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Mount?)null);

            // Act
            var result = await _controller.DeleteMountAsync(Guid.Empty);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}