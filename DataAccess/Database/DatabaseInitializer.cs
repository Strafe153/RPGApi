using Autofac;
using DataAccess.Extensions;
using DbUp;

namespace DataAccess.Database;

public static class DatabaseInitializer
{
    public static void Initialize(string connectionString, ILifetimeScope lifetimeScope)
    {
        EnsureDatabase.For.PostgresqlDatabase(connectionString);

        var upgrader = DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithScriptsAndCodeWithInjectionEmbeddedInAssembly(typeof(DatabaseInitializer).Assembly, lifetimeScope)
            .WithTransaction()
            .LogToConsole()
            .Build();

        if (upgrader.IsUpgradeRequired())
        {
            upgrader.PerformUpgrade();
        }
    }
}