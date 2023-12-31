using NLog.Web;

namespace WebApi.Configurations;

public static class LoggerConfiguration
{
    public static void ConfigureLoggers(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Host.UseNLog();
    }
}
