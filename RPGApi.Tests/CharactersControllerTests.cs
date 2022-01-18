namespace RPGApi.Tests
{
    public class CharactersControllerTests
    {
        private static readonly Mock<IControllerRepository<Character>> _charRepo = new();
        private static readonly Mock<IControllerRepository<Weapon>> _weaponRepo = new();
        private static readonly Mock<IControllerRepository<Spell>> _spellRepo = new();
        private static readonly Mock<IControllerRepository<Mount>> _mountRepo = new();
        private static readonly Mock<IMapper> _mapper = new();
        private static readonly CharactersController _controller = new(_charRepo.Object,
            _weaponRepo.Object, _spellRepo.Object, _mountRepo.Object, _mapper.Object);

        [Fact]
        public async Task GetAllCharactersAsync_Items_ReturnsActionResultOfReadDtos()
        {
            // Arrange
            _charRepo.Setup(cr => cr.GetAllAsync()).ReturnsAsync(new List<Character>());

            // Act
            var result = await _controller.GetAllCharactersAsync();

            // Assert
            Assert.IsType<ActionResult<IEnumerable<CharacterReadDto>>>(result);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPaginatedCharactersAsync_Items_ReturnsActionResultOfPageDto()
        {
            // Arrange
            _charRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Character>());

            // Act
            var result = await _controller.GetPaginatedCharactersAsync(It.IsAny<int>());

            // Assert
            Assert.IsType<ActionResult<PageDto<CharacterReadDto>>>(result);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetCharacterAsync_ExistingCharacter_ReturnsActionResultOfReadDto()
        {
            // Arrange
            _charRepo.Setup(cr => cr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Character());

            // Act
            var result = await _controller.GetCharacterAsync(Guid.Empty);

            // Assert
            Assert.IsType<ActionResult<CharacterReadDto>>(result);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetCharacterAsync_NonexistingCharacter_ReturnsNotFoundResult()
        {
            // Arrange
            _charRepo.Setup(cr => cr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Character?)null);

            // Act
            var result = await _controller.GetCharacterAsync(Guid.Empty);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateCharacterAsync_ValidData_ReturnsActionResultOfReadDto()
        {
            // Arrange
            _mapper.Setup(m => m.Map<Character>(It.IsAny<CharacterCreateDto>()))
                .Returns(new Character() { Player = new Player() });
            _mapper.Setup(m => m.Map<CharacterReadDto>(It.IsAny<Character>()))
                .Returns(new CharacterReadDto());

            // Act
            var result = await _controller.CreateCharacterAsync(new CharacterCreateDto());

            // Assert
            Assert.IsType<ActionResult<CharacterReadDto>>(result);
            Assert.IsType<CreatedAtActionResult>(result.Result);
        }

        [Fact]
        public async Task UpdateCharacterAsync_ExistingCharacter_ReturnsNoContentResult()
        {
            // Arrange
            _charRepo.Setup(cr => cr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Character());

            // Act
            var result = await _controller.UpdateCharacterAsync(Guid.Empty, new CharacterUpdateDto());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateCharacterAsync_NonexistingCharacter_ReturnsNotFoundResult()
        {
            // Arrange
            _charRepo.Setup(cr => cr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Character?)null);

            // Act
            var result = await _controller.UpdateCharacterAsync(Guid.Empty, new CharacterUpdateDto());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task PartialUpdateCharacterAsync_ExistingCharacter_ReturnsNoContentResult()
        {
            // Arrange
            _charRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Character());
            _mapper.Setup(m => m.Map<CharacterUpdateDto>(It.IsAny<Character>()))
                .Returns(new CharacterUpdateDto());

            Utility.MockObjectModelValidator(_controller);

            // Act
            var result = await _controller.PartialUpdateCharacterAsync(Guid.Empty,
                new JsonPatchDocument<CharacterUpdateDto>());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PartialUpdateCharacterAsync_NonexistingCharacter_ReturnsNotFoundResult()
        {
            // Arrange
            _charRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Character?)null);

            // Act
            var result = await _controller.PartialUpdateCharacterAsync(Guid.Empty,
                new JsonPatchDocument<CharacterUpdateDto>());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteCharacterAsync_ExistingCharacter_ReturnsNoContentResult()
        {
            // Arrange
            _charRepo.Setup(cr => cr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Character());

            // Act
            var result = await _controller.DeleteCharacterAsync(Guid.Empty);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteCharacterAsync_NonexistingCharacter_ReturnsNotFoundResult()
        {
            // Arrange
            _charRepo.Setup(cr => cr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Character?)null);

            // Act
            var result = await _controller.DeleteCharacterAsync(Guid.Empty);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AddWeaponAsync_ExistingCharacterExistingWeapon_ReturnsNoContentResult()
        {
            // Arrange
            _charRepo.Setup(cr => cr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(
                new Character()
                { 
                    Weapons = new List<Weapon>()
                });
            _weaponRepo.Setup(wr => wr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Weapon());

            // Act
            var result = await _controller.AddWeaponAsync(new CharacterAddRemoveItem());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task AddWeaponAsync_ExistingCharacterNonexistingWeapon_ReturnsBadRequestResult()
        {
            // Arrange
            _charRepo.Setup(cr => cr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Character());
            _weaponRepo.Setup(wr => wr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Weapon?)null);

            // Act
            var result = await _controller.AddWeaponAsync(new CharacterAddRemoveItem());

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task AddWeaponAsync_NonexistingCharacter_ReturnsNotFoundResult()
        {
            // Arrange
            _charRepo.Setup(cr => cr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Character?)null);

            // Act
            var result = await _controller.AddWeaponAsync(new CharacterAddRemoveItem());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task RemoveWeaponAsync_ExistingCharacterExistingWeapon_ReturnsNoContentResult()
        {
            // Arrange
            _charRepo.Setup(cr => cr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(
                new Character()
                {
                    Weapons = new List<Weapon>()
                });
            _weaponRepo.Setup(wr => wr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Weapon());

            // Act
            var result = await _controller.RemoveWeaponAsync(new CharacterAddRemoveItem());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task RemoveWeaponAsync_ExistingCharacterNonexistingWeapon_ReturnsBadRequestResult()
        {
            // Arrange
            _charRepo.Setup(cr => cr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Character());
            _weaponRepo.Setup(wr => wr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Weapon?)null);

            // Act
            var result = await _controller.RemoveWeaponAsync(new CharacterAddRemoveItem());

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task RemoveWeaponAsync_NonexistingCharacter_ReturnsNotFoundResult()
        {
            // Arrange
            _charRepo.Setup(cr => cr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Character?)null);

            // Act
            var result = await _controller.RemoveWeaponAsync(new CharacterAddRemoveItem());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AddSpellAsync_ExistingCharacterExistingSpell_ReturnsNoContentResult()
        {
            // Arrange
            _charRepo.Setup(cr => cr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(
                new Character()
                {
                    Spells = new List<Spell>()
                });
            _spellRepo.Setup(sr => sr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Spell());

            // Act
            var result = await _controller.AddSpellAsync(new CharacterAddRemoveItem());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task AddSpellAsync_ExistingCharacterNonexistingSpell_ReturnsBadRequestResult()
        {
            // Arrange
            _charRepo.Setup(cr => cr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Character());
            _spellRepo.Setup(sr => sr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Spell?)null);

            // Act
            var result = await _controller.AddSpellAsync(new CharacterAddRemoveItem());

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task AddSpellAsync_NonexistingCharacter_ReturnsBadRequestResult()
        {
            // Arrange
            _charRepo.Setup(cr => cr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Character?)null);

            // Act
            var result = await _controller.AddSpellAsync(new CharacterAddRemoveItem());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task RemoveSpellAsync_ExistingCharacterExistingSpell_ReturnsNoContentResult()
        {
            // Arrange
            _charRepo.Setup(cr => cr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(
                new Character()
                {
                    Spells = new List<Spell>()
                });
            _spellRepo.Setup(sr => sr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Spell());

            // Act
            var result = await _controller.RemoveSpellAsync(new CharacterAddRemoveItem());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task RemoveSpellAsync_ExistingCharacterNonexistingSpell_ReturnsBadRequestResult()
        {
            // Arrange
            _charRepo.Setup(cr => cr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Character());
            _spellRepo.Setup(sr => sr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Spell?)null);

            // Act
            var result = await _controller.RemoveSpellAsync(new CharacterAddRemoveItem());

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task RemoveSpellAsync_NonexistingCharacter_ReturnsNotFoundResult()
        {
            // Arrange
            _charRepo.Setup(cr => cr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Character?)null);

            // Act
            var result = await _controller.RemoveSpellAsync(new CharacterAddRemoveItem());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AddMountAsync_ExistingCharacterExistingMount_ReturnsNoContentResult()
        {
            // Arrange
            _charRepo.Setup(cr => cr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(
                new Character()
                {
                    Mounts = new List<Mount>()
                });
            _mountRepo.Setup(mr => mr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Mount());

            // Act
            var result = await _controller.AddMountAsync(new CharacterAddRemoveItem());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task AddMountAsync_ExistingCharacterNonexistingMount_ReturnsBadRequestResult()
        {
            // Arrange
            _charRepo.Setup(cr => cr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Character());
            _mountRepo.Setup(mr => mr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Mount?)null);

            // Act
            var result = await _controller.AddMountAsync(new CharacterAddRemoveItem());

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task AddMountAsync_NonexistingCharacter_ReturnsBadRequestResult()
        {
            // Arrange
            _charRepo.Setup(cr => cr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Character?)null);

            // Act
            var result = await _controller.AddMountAsync(new CharacterAddRemoveItem());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task RemoveMountAsync_ExistingCharacterExistingMount_ReturnsNoContentResult()
        {
            // Arrange
            _charRepo.Setup(cr => cr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(
                new Character()
                {
                    Mounts = new List<Mount>()
                });
            _mountRepo.Setup(mr => mr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Mount());

            // Act
            var result = await _controller.RemoveMountAsync(new CharacterAddRemoveItem());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task RemoveMountAsync_ExistingCharacterNonexistingMount_ReturnsBadRequestResult()
        {
            // Arrange
            _charRepo.Setup(cr => cr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Character());
            _mountRepo.Setup(mr => mr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Mount?)null);

            // Act
            var result = await _controller.RemoveMountAsync(new CharacterAddRemoveItem());

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task RemoveMountAsync_NonexistingCharacter_ReturnsNotFoundResult()
        {
            // Arrange
            _charRepo.Setup(cr => cr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Character?)null);

            // Act
            var result = await _controller.RemoveMountAsync(new CharacterAddRemoveItem());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}