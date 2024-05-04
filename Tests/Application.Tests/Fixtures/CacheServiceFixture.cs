using Application.Services;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Bogus;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Application.Tests.Fixtures;

public class CacheServiceFixture
{
    public CacheServiceFixture()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

        Key = new Faker().Random.String();

        var playerFaker = new Faker<Player>()
            .RuleFor(p => p.Id, f => f.Random.Int())
            .RuleFor(p => p.Name, Key)
            .RuleFor(p => p.Role, f => (PlayerRole)f.Random.Int(Enum.GetValues(typeof(PlayerRole)).Length))
            .RuleFor(p => p.PasswordHash, f => f.Random.Bytes(32))
            .RuleFor(p => p.PasswordSalt, f => f.Random.Bytes(32))
            .RuleFor(p => p.RefreshToken, f => f.Random.String(64))
            .RuleFor(p => p.RefreshTokenExpiryDate, f => f.Date.Future());

        DistributedCache = fixture.Freeze<IDistributedCache>();
        Logger = fixture.Freeze<ILogger<CacheService>>();

        CacheService = new CacheService(
            DistributedCache,
            Logger);

        Bytes = new byte[]
        { 
            123, 
            125 
        };

        CacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60),
            SlidingExpiration = TimeSpan.FromSeconds(10)
        };

        Player = playerFaker.Generate();
    }

    public CacheService CacheService { get; }
    public IDistributedCache DistributedCache { get; }
    public ILogger<CacheService> Logger { get; }

    public byte[] Bytes { get; }
    public string Key { get; }
    public DistributedCacheEntryOptions CacheOptions { get; }
    public Player Player { get; }
}
