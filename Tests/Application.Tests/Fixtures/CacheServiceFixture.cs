using Application.Services;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Core.Entities;
using Core.Enums;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Application.Tests.Fixtures;

public class CacheServiceFixture
{
    public CacheServiceFixture()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

        DistributedCache = fixture.Freeze<IDistributedCache>();
        Logger = fixture.Freeze<ILogger<CacheService>>();

        CacheService = new(
            DistributedCache,
            Logger);

        Key = "key";
        Bytes = new byte[] { 123, 125 };
        Player = GetPlayer();
    }

    public CacheService CacheService { get; }
    public IDistributedCache DistributedCache { get; }
    public ILogger<CacheService> Logger { get; }

    public byte[] Bytes { get; }
    public string Key { get; }
    public Player Player { get; }

    private Player GetPlayer()
    {
        return new()
        {
            Id = 1,
            Name = Key,
            Role = PlayerRole.Player,
            PasswordHash = Bytes,
            PasswordSalt = Bytes
        };
    }
}
