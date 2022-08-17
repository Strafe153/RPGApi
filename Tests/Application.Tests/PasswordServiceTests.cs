using Application.Tests.Fixtures;
using Core.Exceptions;
using Moq;
using Xunit;

namespace Application.Tests
{
    public class PasswordServiceTests : IClassFixture<PasswordServiceFixture>
    {
        private readonly PasswordServiceFixture _fixture;

        public PasswordServiceTests(PasswordServiceFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void CreatePasswordHash_ValidData_ReturnsVoid()
        {
            // Act
            var result = () => _fixture.MockPasswordService
                .CreatePasswordHash(_fixture.StringPlaceholder!, out byte[] hash, out byte[] salt);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void CreateToken_ValidData_ReturnsJsonWebToken()
        {
            // Arrange
            _fixture.MockConfigurationSection
                .Setup(cs => cs.Value)
                .Returns(_fixture.StringPlaceholder!);

            _fixture.MockConfiguration
                .Setup(c => c.GetSection(It.IsAny<string>()))
                .Returns(_fixture.MockConfigurationSection.Object);

            // Act
            var result = _fixture.MockPasswordService.CreateToken(_fixture.Player);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<string>(result);
        }

        [Fact]
        public void VerifyPasswordHash_ValidPassword_ReturnsVoid()
        {
            // Act
            var result = () => _fixture.MockPasswordService
                .VerifyPasswordHash(_fixture.StringPlaceholder!, _fixture.PasswordHash, _fixture.Bytes);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void VerifyPasswordHash_InvalidPassword_ThrowsIncorrectPasswordException()
        {
            // Act
            var result = () => _fixture.MockPasswordService
                .VerifyPasswordHash(_fixture.StringPlaceholder!, _fixture.Bytes, _fixture.Bytes);

            // Assert
            Assert.NotNull(result);
            Assert.Throws<IncorrectPasswordException>(result);
        }
    }
}
