using Microsoft.AspNetCore.Mvc;
using RPGApi.Dtos;
using RPGApi.Models;
using RPGApi.Controllers;
using RPGApi.Repositories;
using Moq;
using Xunit;
using AutoMapper;

namespace RPGApi.Tests
{
    public class WeaponsControllerTests
    {
        private static readonly Mock<IControllerRepository<Weapon>> _repo = new();
        private static readonly Mock<IMapper> _mapper = new();
        private static readonly WeaponsController _controller = new(_repo.Object, _mapper.Object);

        [Fact]
        public async Task GetWeaponsAsync_ExistingItems_ReturnsActionResultOfReadDtos()
        {
            // Arrange
            _repo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Weapon>());

            // Act
            var result = await _controller.GetWeaponsAsync();

            // Assert
            Assert.IsType<ActionResult<IEnumerable<WeaponReadDto>>>(result);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetWeaponAsync_ExistingWeapon_ReturnsActionResultOfReadDto()
        {
            // Arrange
            _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Weapon());

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
            _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Weapon)null);

            // Act
            var result = await _controller.GetWeaponAsync(Guid.Empty);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateWeaponAsync_ValidData_ReturnsActionResultOfReadDto()
        {
            // Arrange
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
            _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Weapon());

            // Act
            var result = await _controller.UpdateWeaponAsync(Guid.Empty, new WeaponCreateUpdateDto());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateWeaponAsync_NonexistingWeapon_ReturnsNotFoundResult()
        {
            // Arrange
            _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Weapon)null);

            // Act
            var result = await _controller.UpdateWeaponAsync(Guid.Empty, new WeaponCreateUpdateDto());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteWeaponAsync_ExistingWeapon_ReturnsNoContentResult()
        {
            // Arrange
            _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Weapon());

            // Act
            var result = await _controller.DeleteWeaponAsync(Guid.Empty);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteWeaponAsync_NonexistingWeapon_ReturnsNotFoundResult()
        {
            // Arrange
            _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Weapon)null);

            // Act
            var result = await _controller.DeleteWeaponAsync(Guid.Empty);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}