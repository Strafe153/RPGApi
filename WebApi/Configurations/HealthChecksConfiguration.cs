using Domain.Constants;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace WebApi.Configurations;

public static class HealthChecksConfiguration
{
    public static void ConfigureHealthChecks(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString(ConnectionStringConstants.DatabaseConnection)!)
            .AddRedis(configuration.GetConnectionString(ConnectionStringConstants.RedisConnection)!);

    public static void UseHealthChecks(this WebApplication application) =>
        application.MapHealthChecks("/_health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
}
