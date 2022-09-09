using Application.Tests.Fixtures;
using Core.Entities;
using Core.Exceptions;
using Core.Models;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;

namespace Application.Tests
{
    [TestFixture]
    public class SpellServiceTests
    {
        private SpellServiceFixture _fixture = default!;

        [OneTimeSetUp]
        public void SetUp()
        {
            _fixture = new SpellServiceFixture();
        }

        [Test]
        public async Task GetAllAsync_ValidParameters_ReturnsPaginatedListOfSpell()
        {
            // Arrange
            _fixture.SpellRepository
                .GetAllAsync(Arg.Any<int>(), Arg.Any<int>())
                .Returns(_fixture.PaginatedList);

            // Act
            var result = await _fixture.SpellService.GetAllAsync(_fixture.Id, _fixture.Id);

            // Assert
            result.Should().NotBeNull().And.NotBeEmpty().And.BeOfType<PaginatedList<Spell>>();
        }

        [Test]
        public async Task GetByIdAsync_ExistingSpell_ReturnsSpell()
        {
            // Arrange
            _fixture.SpellRepository
                .GetByIdAsync(Arg.Any<int>())
                .Returns(_fixture.Spell);

            // Act
            var result = await _fixture.SpellService.GetByIdAsync(_fixture.Id);

            // Assert
            result.Should().NotBeNull().And.BeOfType<Spell>();
        }

        [Test]
        public async Task GetByIdAsync_NonexistingSpell_ThrowsNullReferenceException()
        {
            // Arrange
            _fixture.SpellRepository
                .GetByIdAsync(Arg.Any<int>())
                .ReturnsNull();

            // Act
            var result = async () => await _fixture.SpellService.GetByIdAsync(_fixture.Id);

            // Assert
            await result.Should().ThrowAsync<NullReferenceException>();
        }

        [Test]
        public void AddAsync_ValidSpell_ReturnsTask()
        {
            // Act
            var result = _fixture.SpellService.AddAsync(_fixture.Spell);

            // Assert
            result.Should().NotBeNull();
        }

        [Test]
        public void UpdateAsync_ValidSpell_ReturnsTask()
        {
            // Act
            var result = _fixture.SpellService.UpdateAsync(_fixture.Spell);

            // Assert
            result.Should().NotBeNull();
        }

        [Test]
        public void DeleteAsync_ValidSpell_ReturnsTask()
        {
            // Act
            var result = _fixture.SpellService.DeleteAsync(_fixture.Spell);

            // Assert
            result.Should().NotBeNull();
        }

        [Test]
        public void AddToCharacter_ValidData_ReturnsVoid()
        {
            // Act
            var result = () => _fixture.SpellService.AddToCharacter(_fixture.Character, _fixture.Spell);

            // Assert
            result.Should().NotBeNull();
        }

        [Test]
        public void RemoveFromCharacter_ExistingSpell_ReturnsVoid()
        {
            // Act
            var result = () => _fixture.SpellService.RemoveFromCharacter(_fixture.Character, _fixture.Spell);

            // Assert
            result.Should().NotBeNull();
        }

        [Test]
        public void RemoveFromCharacter_NonexistingSpell_ThrowsItemNotFoundException()
        {
            // Act
            var result = () => _fixture.SpellService.RemoveFromCharacter(_fixture.Character, _fixture.Spell);

            // Assert
            result.Should().Throw<ItemNotFoundException>();
        }
    }
}
