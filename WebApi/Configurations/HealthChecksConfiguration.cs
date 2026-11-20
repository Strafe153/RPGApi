using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using WebApi.Constants;

namespace WebApi.Configurations;

public static class HealthChecksConfiguration
{
    public static void ConfigureHealthChecks(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString(ConnectionConstants.DatabaseConnection)!)
            .AddRedis(configuration.GetConnectionString(ConnectionConstants.RedisConnection)!);

    public static void UseHealthChecks(this WebApplication application) =>
        application.MapHealthChecks(
            "/health",
            new()
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
}
