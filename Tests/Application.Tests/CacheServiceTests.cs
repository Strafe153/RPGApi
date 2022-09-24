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

    [OneTimeSetUp]
    public void SetUp()
    {
        _fixture = new();
    }

    [Test]
    public async Task GetAsync_ExistingData_ReturnsData()
    {
        // Arrange
        _fixture.DistributedCache
            .GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(_fixture.Bytes);

        // Act
        var result = await _fixture.CacheService.GetAsync<Player>(_fixture.Key);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public async Task GetAsync_NonexistingData_ReturnsTask()
    {
        // Arrange
        _fixture.DistributedCache
            .GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((byte[])null!);

        // Act
        var result = await _fixture.CacheService.GetAsync<Player>(_fixture.Key);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void SetAsync_ValidData_ReturnsData()
    {
        // Act
        var result = async () => await _fixture.CacheService.SetAsync<Player>(_fixture.Key, _fixture.Player);

        // Assert
        result.Should().NotBeNull();
    }
}
