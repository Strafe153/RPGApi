namespace WebApi.Middleware;

public static class MiddlewareConfiguration
{
    public static void AddApplicationMiddleware(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<ExceptionsMiddleware>();
    }
}
