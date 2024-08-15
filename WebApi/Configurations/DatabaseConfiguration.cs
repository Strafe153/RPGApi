using DataAccess.Database;
using Domain.Shared;

namespace WebApi.Configurations;

public static class DatabaseConfiguration
{
    public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<DatabaseConnectionProvider>();
        services.Configure<AdminOptions>(configuration.GetSection(AdminOptions.SectionName));
    }
}
