using Core.Dtos;
using Core.Dtos.SpellDtos;
using Core.Entities;
using Core.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Tests.Fixtures;
using Xunit;

namespace WebApi.Tests
{
    public class SpellsControllerTests : IClassFixture<SpellsControllerFixture>
    {
        private readonly SpellsControllerFixture _fixture;

        public SpellsControllerTests(SpellsControllerFixture fixture)
        {
            _fixture = fixture;

            _fixture.MockControllerBaseUser();
            _fixture.MockObjectModelValidator(_fixture.MockSpellsController);
        }

        [Fact]
        public async Task GetAsync_ValidPageParameters_ReturnsActionResultOfPageDtoOfSpellReadDto()
        {
            // Arrange
            _fixture.MockSpellService
                .Setup(s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(_fixture.PaginatedList);

            _fixture.MockPaginatedMapper
                .Setup(m => m.Map(It.IsAny<PaginatedList<Spell>>()))
                .Returns(_fixture.PageDto);

            // Act
            var result = await _fixture.MockSpellsController.GetAsync(_fixture.PageParameters);
            var pageDto = result.Result.As<OkObjectResult>().Value.As<PageDto<SpellReadDto>>();

            // Assert
            result.Should().NotBeNull().And.BeOfType<ActionResult<PageDto<SpellReadDto>>>();
            pageDto.Entities.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetAsync_ExistingSpell_ReturnsActionResultOfSpellReadDto()
        {
            // Arrange
            _fixture.MockSpellService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Spell);

            _fixture.MockReadMapper
                .Setup(m => m.Map(It.IsAny<Spell>()))
                .Returns(_fixture.SpellReadDto);

            // Act
            var result = await _fixture.MockSpellsController.GetAsync(_fixture.Id);
            var readDto = result.Result.As<OkObjectResult>().Value.As<SpellReadDto>();

            // Assert
            result.Should().NotBeNull().And.BeOfType<ActionResult<SpellReadDto>>();
            readDto.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateAsync_ValidDto_ReturnsActionResultOfSpellReadDto()
        {
            // Arrange
            _fixture.MockCreateMapper
                .Setup(m => m.Map(It.IsAny<SpellBaseDto>()))
                .Returns(_fixture.Spell);

            // Act
            var result = await _fixture.MockSpellsController.CreateAsync(_fixture.SpellBaseDto);
            var readDto = result.Result.As<CreatedAtActionResult>().Value.As<SpellReadDto>();

            // Assert
            result.Should().NotBeNull().And.BeOfType<ActionResult<SpellReadDto>>();
            readDto.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateAsync_ExistingSpellValidDto_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockSpellService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Spell);

            // Act
            var result = await _fixture.MockSpellsController.UpdateAsync(_fixture.Id, _fixture.SpellBaseDto);

            // Assert
            result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UpdateAsync_ExistingSpellValidPatchDocument_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockSpellService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Spell);

            _fixture.MockUpdateMapper
                .Setup(m => m.Map(_fixture.Spell))
                .Returns(_fixture.SpellBaseDto);

            // Act
            var result = await _fixture.MockSpellsController.UpdateAsync(_fixture.Id, _fixture.PatchDocument);

            // Assert
            result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UpdateAsync_ExistingSpellInvalidPatchDocument_ReturnsObjectResult()
        {
            // Arrange
            _fixture.MockSpellService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Spell);

            _fixture.MockUpdateMapper
                .Setup(m => m.Map(_fixture.Spell))
                .Returns(_fixture.SpellBaseDto);

            _fixture.MockModelError(_fixture.MockSpellsController);

            // Act
            var result = await _fixture.MockSpellsController.UpdateAsync(_fixture.Id, _fixture.PatchDocument);

            // Assert
            result.Should().NotBeNull().And.BeOfType<ObjectResult>();
        }

        [Fact]
        public async Task DeleteAsync_ExistingSpell_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockSpellService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Spell);

            // Act
            var result = await _fixture.MockSpellsController.DeleteAsync(_fixture.Id);

            // Assert
            result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task HitAsync_ValidDto_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.MockCharacterService
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Character);

            _fixture.MockCharacterService
                .Setup(s => s.GetSpell(It.IsAny<Character>(), It.IsAny<int>()))
                .Returns(_fixture.Spell);

            // Act
            var result = await _fixture.MockSpellsController.HitAsync(_fixture.HitDto);

            // Assert
            result.Should().NotBeNull().And.BeOfType<NoContentResult>();
        }
    }
}
