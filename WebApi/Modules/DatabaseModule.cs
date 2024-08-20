using Autofac;
using DataAccess.Database;
using Domain.Shared;

namespace WebApi.Modules;

public class DatabaseModule : Module
{
    private readonly IConfiguration _configuration;

    public DatabaseModule(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void Load(ContainerBuilder builder)
    {
        var adminOptions = _configuration.GetSection(AdminOptions.SectionName).Get<AdminOptions>()!;

        builder
            .RegisterInstance(adminOptions)
            .SingleInstance();

        builder
            .RegisterType<DatabaseConnectionProvider>()
            .SingleInstance();
    }
}
