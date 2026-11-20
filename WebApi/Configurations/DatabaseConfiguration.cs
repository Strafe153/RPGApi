using DataAccess.Database;
using DataAccess.Shared;
using WebApi.Constants;

namespace WebApi.Configurations;

public static class DatabaseConfiguration
{
    public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(_ =>
        {
            var connectionString = configuration.GetConnectionString(ConnectionConstants.DatabaseConnection)!;
            return new DatabaseConnectionProvider(connectionString);
        });

        services.Configure<AdminOptions>(configuration.GetSection(AdminOptions.SectionName));
    }
}
