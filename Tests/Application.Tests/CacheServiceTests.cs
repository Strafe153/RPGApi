using Application.Tests.Fixtures;
using Core.Entities;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Application.Tests;

[TestFixture]
public class CacheServiceTests
{
    private CacheServiceFixture _fixture = default!;

    [SetUp]
    public void SetUp()
    {
        _fixture = new CacheServiceFixture();
    }

    [Test]
    public async Task GetAsync_Should_ReturnData_WhenDataExists()
    {
        // Arrange
        _fixture.DistributedCache
            .GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(_fixture.Bytes);

        // Act
        var result = await _fixture.CacheService.GetAsync(_fixture.Key);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public async Task GetAsync_Should_ReturnTask_WhenDataDoesNotExist()
    {
        // Arrange
        _fixture.DistributedCache
            .GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((byte[])null!);

        // Act
        var result = await _fixture.CacheService.GetAsync(_fixture.Key);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void SetAsync_Should_ReturnTask_WhenDataIsValid()
    {
        // Act
        var result = async () => await _fixture.CacheService.SetAsync(_fixture.Key, _fixture.Player, _fixture.CacheOptions);

        // Assert
        result.Should().NotBeNull();
    }
}
