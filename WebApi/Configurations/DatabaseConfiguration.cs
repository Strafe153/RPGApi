using DataAccess;

namespace WebApi.Configurations;

public static class DatabaseConfiguration
{
    public static void ConfigureDatabase(this IServiceCollection services)
    {
        services.AddSingleton<RPGContext>();
    }
}
