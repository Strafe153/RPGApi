using Application.Helpers;
using Autofac;
using Domain.Constants;
using Domain.Helpers;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;

namespace WebApi.Modules;

public class CachingModule : Module
{
    private readonly IConfiguration _configuration;

    public CachingModule(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void Load(ContainerBuilder builder)
    {
        var redisOptions = _configuration.GetConnectionString(ConnectionStringConstants.RedisConnection);

        builder
            .Register(_ =>
            {
                RedisCacheOptions cacheOptions = new()
                {
                    Configuration = redisOptions
                };

                return new RedisCache(cacheOptions);
            })
            .As<IDistributedCache>()
            .SingleInstance();

        builder
            .RegisterType<CacheHelper>()
            .As<ICacheHelper>()
            .SingleInstance();
    }
}
