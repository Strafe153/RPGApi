using Application.Tests.Fixtures;
using Core.Exceptions;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Application.Tests;

[TestFixture]
public class PasswordServiceTests
{
    private PasswordServiceFixture _fixture = default!;

    [OneTimeSetUp]
    public void SetUp()
    {
        _fixture = new();
    }

    [Test]
    public void CreatePasswordHash_ValidPassword_ReturnsVoid()
    {
        // Act
        var result = () => _fixture.PasswordService.CreatePasswordHash(_fixture.StringPlaceholder!);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void CreateToken_ValidPlayer_ReturnsString()
    {
        // Arrange
        _fixture.ConfigurationSection.Value
            .Returns(_fixture.StringPlaceholder!);

        _fixture.Configuration.GetSection(Arg.Any<string>())
            .Returns(_fixture.ConfigurationSection);

        // Act
        var result = _fixture.PasswordService.CreateToken(_fixture.Player);

        // Assert
        result.Should().NotBeNull().And.BeOfType<string>();
    }

    [Test]
    public void VerifyPasswordHash_ValidParameters_ReturnsVoid()
    {
        // Act
        var result = () => _fixture.PasswordService
            .VerifyPasswordHash(_fixture.StringPlaceholder!, _fixture.PasswordHash, _fixture.Bytes);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void VerifyPasswordHash_InvalidParameters_ThrowsIncorrectPasswordException()
    {
        // Act
        var result = () => _fixture.PasswordService
            .VerifyPasswordHash(_fixture.StringPlaceholder!, _fixture.Bytes, _fixture.Bytes);

        // Assert
        result.Should().Throw<IncorrectPasswordException>();
    }
}
