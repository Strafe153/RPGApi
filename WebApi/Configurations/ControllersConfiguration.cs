using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WebApi.Filters;

namespace WebApi.Configurations;

public static class ControllersConfiguration
{
    public static void ConfigureControllers(this IServiceCollection services) =>
        services
            .AddControllers(options =>
            {
                options.Filters.Add<ExceptionHandlingFilterAttribute>();
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
}