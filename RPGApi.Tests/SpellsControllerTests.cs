namespace RPGApi.Tests
{
    public class SpellsControllerTests
    {
        private static readonly Mock<IControllerRepository<Spell>> _spellRepo = new();
        private static readonly Mock<IControllerRepository<Character>> _charRepo = new();
        private static readonly Mock<IMapper> _mapper = new();
        private static readonly SpellsController _controller = new(
            _spellRepo.Object, _charRepo.Object, _mapper.Object);

        private static readonly Character _character = new()
        {
            Spells = new List<Spell>()
            {
                new Spell()
                {
                    Id = Guid.Empty
                }
            }
        };

        [Fact]
        public async Task GetAllSpellsAsync_Items_ReturnsActionResultOfReadDtos()
        {
            // Arrange
            _spellRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Spell>());

            // Act
            var result = await _controller.GetAllSpellsAsync();

            // Assert
            Assert.IsType<ActionResult<IEnumerable<SpellReadDto>>>(result);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPaginatedSpellsAsync_Items_ReturnsActionResultOfPageDto()
        {
            // Arrange
            _spellRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Spell>());

            // Act
            var result = await _controller.GetPaginatedSpellsAsync(It.IsAny<int>());

            // Assert
            Assert.IsType<ActionResult<PageDto<SpellReadDto>>>(result);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetSpellAsync_ExistingSpell_ReturnsActionResultOfReadDto()
        {
            // Arrange
            _spellRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Spell());

            // Act
            var result = await _controller.GetSpellAsync(Guid.Empty);

            // Assert
            Assert.IsType<ActionResult<SpellReadDto>>(result);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetSpellAsync_NonexistingSpell_ReturnsNotFoundResult()
        {
            // Arrange
            _spellRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Spell?)null);

            // Act
            var result = await _controller.GetSpellAsync(Guid.Empty);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateSpellAsync_ValidData_ReturnsActionResultOfReadDto()
        {
            // Arrange
            _mapper.Setup(m => m.Map<Spell>(It.IsAny<SpellCreateUpdateDto>()))
                .Returns(new Spell() { Name = "test_name" });
            _mapper.Setup(m => m.Map<SpellReadDto>(It.IsAny<Spell>()))
                .Returns(new SpellReadDto());

            // Act
            var result = await _controller.CreateSpellAsync(new SpellCreateUpdateDto());

            // Assert
            Assert.IsType<ActionResult<SpellReadDto>>(result);
            Assert.IsType<CreatedAtActionResult>(result.Result);
        }

        [Fact]
        public async Task UpdateSpellAsync_ExistingSpell_ReturnsNoContentResult()
        {
            // Arrange
            _spellRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Spell());

            // Act
            var result = await _controller.UpdateSpellAsync(Guid.Empty, new SpellCreateUpdateDto());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateSpellAsync_NonexistingSpell_ReturnsNotFoundResult()
        {
            // Arrange
            _spellRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Spell?)null);

            // Act
            var result = await _controller.UpdateSpellAsync(Guid.Empty, new SpellCreateUpdateDto());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task PartialUpdateSpellAsync_ExistingSpell_ReturnsNoContentResult()
        {
            // Arrange
            _spellRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Spell());
            _mapper.Setup(m => m.Map<SpellCreateUpdateDto>(It.IsAny<Spell>()))
                .Returns(new SpellCreateUpdateDto());

            Utility.MockObjectModelValidator(_controller);

            // Act
            var result = await _controller.PartialUpdateSpellAsync(Guid.Empty,
                new JsonPatchDocument<SpellCreateUpdateDto>());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PartialUpdateSpellAsync_NonexistingSpell_ReturnsNotFoundResult()
        {
            // Arrange
            _spellRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Spell?)null);

            // Act
            var result = await _controller.PartialUpdateSpellAsync(Guid.Empty,
                new JsonPatchDocument<SpellCreateUpdateDto>());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteSpellAsync_ExistingSpell_ReturnsNoContentResult()
        {
            // Arrange
            _spellRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Spell());

            // Act
            var result = await _controller.DeleteSpellAsync(Guid.Empty);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteSpellAsync_NonexistingSpell_ReturnsNotFoundResult()
        {
            // Arrange
            _spellRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Spell?)null);

            // Act
            var result = await _controller.DeleteSpellAsync(Guid.Empty);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task HitAsync_ValidData_ReturnsNoContentResult()
        {
            // Arrange
            _charRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(_character);

            // Act
            var result = await _controller.HitAsync(new HitDto() { ReceiverId = Guid.Empty });

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task HitAsync_NonexistingCharacter_ReturnsNotFoundObjectResult()
        {
            // Arrange
            _charRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Character?)null);

            // Act
            var result = await _controller.HitAsync(new HitDto());

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task HitAsync_NoAccessRights_ReturnsForbidResult()
        {
            // Arrange
            _charRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(_character);
            Utility.MockUserIdentityName(_controller);

            // Act
            var result = await _controller.HitAsync(new HitDto());

            // Assert
            Assert.IsType<ForbidResult>(result);
        }
    }
}