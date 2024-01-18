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

    [SetUp]
    public void OneTimeSetUp() => _fixture = new TokenServiceFixture();

    [Test]
    public void GenerateAccessToken_Should_ReturnString_WhenDataIsValid()
    {
        // Act
        var result = _fixture.TokenService.GenerateAccessToken(_fixture.PlayerWithValidToken);

        // Assert
        result.Should().NotBeNull().And.BeOfType<string>();
    }

    [Test]
    public void GenerateRefreshToken_Should_ReturnString_WhenDataIsValid()
    {
        // Act
        var result = _fixture.TokenService.GenerateRefreshToken();

        // Assert
        result.Should().NotBeNull().And.BeOfType<string>();
    }

    [Test]
    public void SetRefreshToken_Should_ReturnVoid_WhenDataIsValid()
    {
        // Act
        var result = () => _fixture.TokenService.SetRefreshToken(_fixture.PlayerWithValidToken, _fixture.ValidToken);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void ValidateRefreshToken_Should_ReturnVoid_WhenTokenIsValid()
    {
        // Act
        var result = () => _fixture.TokenService.ValidateRefreshToken(_fixture.PlayerWithValidToken, _fixture.ValidToken);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void ValidateRefreshToken_Should_ThrowInvalidTokenException_WhenTokenIsInvalid()
    {
        // Act
        var result = () => _fixture.TokenService.ValidateRefreshToken(_fixture.PlayerWithValidToken, _fixture.InvalidToken);

        // Assert
        result.Should().Throw<InvalidTokenException>();
    }

    [Test]
    public void ValidateRefreshToken_Should_ThrowInvalidTokenException_WhenTokenIsExpired()
    {
        // Act
        var result = () => _fixture.TokenService.ValidateRefreshToken(_fixture.PlayerWithExpiredToken, _fixture.ValidToken);

        // Assert
        result.Should().Throw<InvalidTokenException>();
    }
}
