using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace WebApi.Configurations;

public static class ControllersConfiguration
{
    public static void ConfigureControllers(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.SuppressAsyncSuffixInActionNames = false;
            options.Filters.Add<ExceptionHandlingFilter>();
        })
        .AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        });
    }
}