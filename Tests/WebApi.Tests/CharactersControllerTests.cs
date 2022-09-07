using Core.Dtos;
using Core.Dtos.CharacterDtos;
using Core.Entities;
using Core.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Tests.Fixtures;
using Xunit;

namespace WebApi.Tests
{
    public class CharactersControllerTests : IClassFixture<CharactersControllerFixture>
    {
        private readonly CharactersControllerFixture _fixture;

        public CharactersControllerTests(CharactersControllerFixture fixture)
        {
            _fixture = fixture;

            _fixture.MockControllerBaseUser();
            _fixture.MockObjectModelValidator(_fixture.MockCharactersController);
        }

        [Fact]
        public async Task GetAsync_ValidPageParameters_ReturnsActionResultOfPageDtoOfCharacterReadDto()
        {
            // Arrange
            _fixture.MockCharacterService
                .Setup(s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(_fixture.PaginatedList);

            _fixture.MockPaginatedMapper
                .Setup(m => m.Map(It.IsAny<PaginatedList<Character>>()))
                .Returns(_fixture.PageDto);

            // Act
            var result = await _fixture.MockCharactersController.GetAsync(_fixture.PageParameters);
            var pageDto = result.Result.As<OkObjectResult>().Value.As<PageDto<CharacterReadDto>>();

            result.Should().NotBeNull().And.BeOfType<ActionResult<PageDto<CharacterReadDto>>>();
            pageDto.Entities.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetAsync_ExistingCharacter_ReturnsActionResultOfCharacterReadDto()
        {
            // Arrange
            _fixture.MockCharacterService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Character);

            _fixture.MockReadMapper
                .Setup(m => m.Map(It.IsAny<Character>()))
                .Returns(_fixture.CharacterReadDto);

            // Act
            var result = await _fixture.MockCharactersController.GetAsync(_fixture.Id);
            var readDto = result.Result.As<OkObjectResult>().Value.As<CharacterReadDto>();

            // Assert
            result.Should().NotBeNull().And.BeOfType<ActionResult<CharacterReadDto>>();
            readDto.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateAsync_ValidDto_ReturnsActionResultOfCharacterReadDto()
        {
            // Arrange
            _fixture.MockCreateMapper
                .Setup(m => m.Map(It.IsAny<CharacterCreateDto>()))
                .Returns(_fixture.Character);

            // Act
            var result = await _fixture.MockCharactersController.CreateAsync(_fixture.CharacterCreateDto);
            var readDto = result.Result.As<CreatedAtActionResult>().Value.As<CharacterReadDto>();

            // Assert
            result.Should().NotBeNull().And.BeOfType<ActionResult<CharacterReadDto>>();
            readDto.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateAsync_ExistingCharacterValidViewModel_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockCharacterService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Character);

            // Act
            var result = await _fixture.MockCharactersController.UpdateAsync(_fixture.Id, _fixture.CharacterUpdateDto);

            // Assert
            result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UpdateAsync_ExistingCharacterValidPatchDocument_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockCharacterService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Character);

            _fixture.MockUpdateMapper
                .Setup(m => m.Map(_fixture.Character))
                .Returns(_fixture.CharacterUpdateDto);

            // Act
            var result = await _fixture.MockCharactersController.UpdateAsync(_fixture.Id, _fixture.PatchDocument);

            // Assert
            result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UpdateAsync_ExistingCharacterInvalidPatchDocument_ReturnsObjectResult()
        {
            // Arrange
            _fixture.MockCharacterService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Character);

            _fixture.MockUpdateMapper
                .Setup(m => m.Map(_fixture.Character))
                .Returns(_fixture.CharacterUpdateDto);

            _fixture.MockModelError(_fixture.MockCharactersController);

            // Act
            var result = await _fixture.MockCharactersController.UpdateAsync(_fixture.Id, _fixture.PatchDocument);

            // Assert
            result.Should().NotBeNull().And.BeOfType<ObjectResult>();
        }

        [Fact]
        public async Task DeleteAsync_ExistingCharacter_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockCharacterService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Character);

            // Act
            var result = await _fixture.MockCharactersController.DeleteAsync(_fixture.Id);

            // Assert
            result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task AddWeaponAsync_ExistingCharacterExistingWeapon_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockCharacterService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Character);

            _fixture.MockWeaponService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Weapon);

            // Act
            var result = await _fixture.MockCharactersController.AddWeaponAsync(_fixture.ItemDto);

            // Assert
            result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task RemoveWeaponAsync_ExistingCharacterExistingWeapon_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockCharacterService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Character);

            _fixture.MockWeaponService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Weapon);

            // Act
            var result = await _fixture.MockCharactersController.RemoveWeaponAsync(_fixture.ItemDto);

            // Assert
            result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task AddSpellAsync_ExistingCharacterExistingSpell_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockCharacterService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Character);

            _fixture.MockSpellService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Spell);

            // Act
            var result = await _fixture.MockCharactersController.AddSpellAsync(_fixture.ItemDto);

            // Assert
            result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task RemoveSpellAsync_ExistingCharacterExistingSpell_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockCharacterService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Character);

            _fixture.MockSpellService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Spell);

            // Act
            var result = await _fixture.MockCharactersController.RemoveSpellAsync(_fixture.ItemDto);

            // Assert
            result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task AddMountAsync_ExistingCharacterExistingMount_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockCharacterService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Character);

            _fixture.MockMountService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Mount);

            // Act
            var result = await _fixture.MockCharactersController.AddMountAsync(_fixture.ItemDto);

            // Assert
            result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task RemoveMountAsync_ExistingCharacterExistingMount_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockCharacterService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Character);

            _fixture.MockMountService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Mount);

            // Act
            var result = await _fixture.MockCharactersController.RemoveMountAsync(_fixture.ItemDto);

            // Assert
            result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        }
    }
}
