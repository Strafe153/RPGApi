using WebApi.Filters;

namespace WebApi.Configurations;

public static class ControllersConfiguration
{
    public static void ConfigureControllers(this IServiceCollection services) =>
        services
            .AddControllers(options =>
            {
                options.Filters.Add<ExceptionHandlingFilterAttribute>();
            });
}
