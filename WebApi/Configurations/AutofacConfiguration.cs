using Autofac.Extensions.DependencyInjection;
using Autofac;
using WebApi.Modules;

namespace WebApi.Configurations;

public static class AutofacConfiguration
{
    public static void ConfigureAutofac(this WebApplicationBuilder builder) =>
        builder.Host
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>(context =>
                context
                    .RegisterModule(new DatabaseModule(builder.Configuration))
                    .RegisterModule(new CachingModule(builder.Configuration))
                    .RegisterModule<PlayersModule>()
                    .RegisterModule<CharactersModule>()
                    .RegisterModule<WeaponsModule>()
                    .RegisterModule<SpellsModule>()
                    .RegisterModule<MountsModule>());
}
