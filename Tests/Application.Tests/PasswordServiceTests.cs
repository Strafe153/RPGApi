using Application.Tests.Fixtures;
using Core.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace Application.Tests;

[TestFixture]
public class PasswordServiceTests
{
    private PasswordServiceFixture _fixture = default!;

    [OneTimeSetUp]
    public void SetUp()
    {
        _fixture = new PasswordServiceFixture();
    }

    [Test]
    public void GeneratePasswordHashAndSalt_ValidPassword_ReturnsTupleOfByteArrays()
    {
        // Act
        var result = _fixture.PasswordService.GeneratePasswordHashAndSalt(_fixture.StringPlaceholder!);

        // Assert
        result.Should().NotBeNull().And.BeOfType<(byte[], byte[])>();
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
