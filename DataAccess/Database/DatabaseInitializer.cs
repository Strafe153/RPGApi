using DataAccess.Extensions;
using DbUp;

namespace DataAccess.Database;

public static class DatabaseInitializer
{
    public static void Initialize(string connectionString, IServiceProvider serviceProvider)
    {
        EnsureDatabase.For.PostgresqlDatabase(connectionString);

        var upgrader = DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithScriptsAndCodeWithInjectionEmbeddedInAssembly(typeof(DatabaseInitializer).Assembly, serviceProvider)
            .WithTransaction()
            .Build();

        if (upgrader.IsUpgradeRequired())
        {
            upgrader.PerformUpgrade();
        }
    }
}