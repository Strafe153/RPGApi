using Application.Tests.Fixtures;
using Core.Exceptions;
using FluentAssertions;
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
        public void CreatePasswordHash_ValidPassword_ReturnsVoid()
        {
            // Act
            var result = () => _fixture.MockPasswordService
                .CreatePasswordHash(_fixture.StringPlaceholder!, out byte[] hash, out byte[] salt);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void CreateToken_ValidPlayer_ReturnsString()
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
            result.Should().NotBeNull();
            result.Should().BeOfType<string>();
        }

        [Fact]
        public void VerifyPasswordHash_ValidParameters_ReturnsVoid()
        {
            // Act
            var result = () => _fixture.MockPasswordService
                .VerifyPasswordHash(_fixture.StringPlaceholder!, _fixture.PasswordHash, _fixture.Bytes);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void VerifyPasswordHash_InvalidParameters_ThrowsIncorrectPasswordException()
        {
            // Act
            var result = () => _fixture.MockPasswordService
                .VerifyPasswordHash(_fixture.StringPlaceholder!, _fixture.Bytes, _fixture.Bytes);

            // Assert
            result.Should().NotBeNull();
            result.Should().Throw<IncorrectPasswordException>();
        }
    }
}
