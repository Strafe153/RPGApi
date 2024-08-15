using DataAccess.Database;
using DbUp.Builder;
using System.Reflection;

namespace DataAccess.Extensions;

public static class UpgradeEngineBuilderExtensions
{
    public static UpgradeEngineBuilder WithScriptsAndCodeWithInjectionEmbeddedInAssembly(
        this UpgradeEngineBuilder builder,
        Assembly assembly,
        IServiceProvider serviceProvider)
    {
        const string SqlExtension = ".sql";

        return builder.WithScripts(
            new EmbeddedScriptAndCodeWithInjectionProvider(
                assembly,
                s => s.EndsWith(SqlExtension, StringComparison.OrdinalIgnoreCase),
                _ => true,
                serviceProvider));
    }
}
