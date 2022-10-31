﻿using Application.Tests.Fixtures;
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
    public void GeneratePasswordHashAndSalt_Should_ReturnTupleOfByteArrays_WhenPasswordIsValid()
    {
        // Act
        var result = _fixture.PasswordService.GeneratePasswordHashAndSalt(_fixture.StringPlaceholder!);

        // Assert
        result.Should().NotBeNull().And.BeOfType<(byte[], byte[])>();
    }

    [Test]
    public void VerifyPasswordHash_Should_ReturnVoid_WhenParametersAreValid()
    {
        // Act
        var result = () => _fixture.PasswordService.VerifyPasswordHash(_fixture.StringPlaceholder!, _fixture.Player);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void VerifyPasswordHash_Should_ThrowIncorrectPasswordException_WhenParametersAreInvalid()
    {
        // Act
        var result = () => _fixture.PasswordService.VerifyPasswordHash(_fixture.StringPlaceholder!, _fixture.Player);

        // Assert
        result.Should().Throw<IncorrectPasswordException>();
    }
}
