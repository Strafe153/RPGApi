using Application.Tests.Fixtures;
using Core.Exceptions;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Application.Tests;

[TestFixture]
public class TokenServiceTests
{
    private TokenServiceFixture _fixture = default!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _fixture = new TokenServiceFixture();
    }

    [Test]
    public void GenerateAccessToken_ValidData_ReturnsString()
    {
        // Arrange
        _fixture.ConfigurationSection.Value
            .Returns(_fixture.ValidToken);

        _fixture.Configuration
            .GetSection(Arg.Any<string>())
            .Returns(_fixture.ConfigurationSection);

        // Act
        var result = _fixture.TokenService.GenerateAccessToken(_fixture.PlayerWithValidToken);

        // Assert
        result.Should().NotBeNull().And.BeOfType<string>();
    }

    [Test]
    public void GenerateRefreshToken_ValidData_ReturnsString()
    {
        // Act
        var result = _fixture.TokenService.GenerateRefreshToken();

        // Assert
        result.Should().NotBeNull().And.BeOfType<string>();
    }

    [Test]
    public void SetRefreshToken_ValidData_ReturnsVoid()
    {
        // Act
        var result = () => _fixture.TokenService.SetRefreshToken(
            _fixture.ValidToken, _fixture.PlayerWithValidToken, _fixture.HttpResponse);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void ValidateRefreshToken_ValidToken_ReturnsVoid()
    {
        // Act
        var result = () => _fixture.TokenService.ValidateRefreshToken(_fixture.PlayerWithValidToken, _fixture.ValidToken);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void ValidateRefreshToken_InvalidToken_ThrowsInvalidTokenException()
    {
        // Act
        var result = () => _fixture.TokenService.ValidateRefreshToken(_fixture.PlayerWithValidToken, _fixture.InvalidToken);

        // Assert
        result.Should().Throw<InvalidTokenException>();
    }

    [Test]
    public void ValidateRefreshToken_ExpiredToken_ThrowsInvalidTokenException()
    {
        // Act
        var result = () => _fixture.TokenService.ValidateRefreshToken(_fixture.PlayerWithExpiredToken, _fixture.ValidToken);

        // Assert
        result.Should().Throw<InvalidTokenException>();
    }
}
