namespace RPGApi.Tests
{
    public class WeaponsControllerTests
    {
        private static readonly Mock<IControllerRepository<Weapon>> _weaponRepo = new();
        private static readonly Mock<IControllerRepository<Character>> _charRepo = new();
        private static readonly Mock<IMapper> _mapper = new();
        private static readonly WeaponsController _controller = new(
            _weaponRepo.Object, _charRepo.Object, _mapper.Object);

        [Fact]
        public async Task GetAllWeaponsAsync_Items_ReturnsActionResultOfReadDtos()
        {
            // Arrange
            _weaponRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Weapon>());

            // Act
            var result = await _controller.GetAllWeaponsAsync();

            // Assert
            Assert.IsType<ActionResult<IEnumerable<WeaponReadDto>>>(result);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPaginatedWeaponsAsync_Items_ReturnsActionResultOfPageDto()
        {
            // Arrange
            _weaponRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Weapon>());

            // Act
            var result = await _controller.GetPaginatedWeaponsAsync(It.IsAny<int>());

            // Assert
            Assert.IsType<ActionResult<PageDto<WeaponReadDto>>>(result);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetWeaponAsync_ExistingWeapon_ReturnsActionResultOfReadDto()
        {
            // Arrange
            _weaponRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Weapon());

            // Act
            var result = await _controller.GetWeaponAsync(Guid.Empty);

            // Assert
            Assert.IsType<ActionResult<WeaponReadDto>>(result);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetWeaponAsync_NonexistingWeapon_ReturnsNotFoundResult()
        {
            // Arrange
            _weaponRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Weapon?)null);

            // Act
            var result = await _controller.GetWeaponAsync(Guid.Empty);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateWeaponAsync_ValidData_ReturnsActionResultOfReadDto()
        {
            // Arrange
            _mapper.Setup(m => m.Map<Weapon>(It.IsAny<WeaponCreateUpdateDto>()))
                .Returns(new Weapon() { Name = "test_name" });
            _mapper.Setup(m => m.Map<WeaponReadDto>(It.IsAny<Weapon>()))
                .Returns(new WeaponReadDto());

            // Act
            var result = await _controller.CreateWeaponAsync(new WeaponCreateUpdateDto());

            // Assert
            Assert.IsType<ActionResult<WeaponReadDto>>(result);
            Assert.IsType<CreatedAtActionResult>(result.Result);
        }

        [Fact]
        public async Task UpdateWeaponAsync_ExistingWeapon_ReturnsNoContentResult()
        {
            // Arrange
            _weaponRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Weapon());

            // Act
            var result = await _controller.UpdateWeaponAsync(Guid.Empty, new WeaponCreateUpdateDto());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateWeaponAsync_NonexistingWeapon_ReturnsNotFoundResult()
        {
            // Arrange
            _weaponRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Weapon?)null);

            // Act
            var result = await _controller.UpdateWeaponAsync(Guid.Empty, new WeaponCreateUpdateDto());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task PartialUpdateWeaponAsync_ExistingWeapon_ReturnsNoContentResult()
        {
            // Arrange
            _weaponRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Weapon());
            _mapper.Setup(m => m.Map<WeaponCreateUpdateDto>(It.IsAny<Weapon>()))
                .Returns(new WeaponCreateUpdateDto());

            Utility.MockObjectModelValidator(_controller);

            // Act
            var result = await _controller.PartialUpdateWeaponAsync(Guid.Empty,
                new JsonPatchDocument<WeaponCreateUpdateDto>());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PartialUpdateWeaponAsync_NonexistingWeapon_ReturnsNotFoundResult()
        {
            // Arrange
            _weaponRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Weapon?)null);

            // Act
            var result = await _controller.PartialUpdateWeaponAsync(Guid.Empty,
                new JsonPatchDocument<WeaponCreateUpdateDto>());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteWeaponAsync_ExistingWeapon_ReturnsNoContentResult()
        {
            // Arrange
            _weaponRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Weapon());

            // Act
            var result = await _controller.DeleteWeaponAsync(Guid.Empty);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteWeaponAsync_NonexistingWeapon_ReturnsNotFoundResult()
        {
            // Arrange
            _weaponRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Weapon?)null);

            // Act
            var result = await _controller.DeleteWeaponAsync(Guid.Empty);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}