using Core.Shared;
using DataAccess;

namespace WebApi.Configurations;

public static class DatabaseConfiguration
{
    public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<RPGContext>();
        services.Configure<AdminOptions>(configuration.GetSection(AdminOptions.SectionName));
    }
}
