using DbUp;
using DbUp.Engine;
using DbUp.Engine.Transactions;
using DbUp.ScriptProviders;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DataAccess.Database;

public class EmbeddedScriptAndCodeWithInjectionProvider : IScriptProvider
{
    private readonly EmbeddedScriptProvider _embeddedScriptProvider;
    private readonly Assembly _assembly;
    private readonly Func<string, bool> _filter;
    private readonly SqlScriptOptions _sqlScriptOptions;
    private readonly IServiceProvider _serviceProvider;

    public EmbeddedScriptAndCodeWithInjectionProvider(
        Assembly assembly,
        Func<string, bool> filter,
        IServiceProvider serviceProvider)
            : this(assembly, filter, filter, new SqlScriptOptions(), serviceProvider)
    {
    }

    public EmbeddedScriptAndCodeWithInjectionProvider(
        Assembly assembly,
        Func<string, bool> filter,
        Func<string, bool> codeScriptFilter,
        IServiceProvider serviceProvider)
            : this(assembly, filter, codeScriptFilter, new SqlScriptOptions(), serviceProvider)
    {
    }

    public EmbeddedScriptAndCodeWithInjectionProvider(
        Assembly assembly,
        Func<string, bool> filter,
        SqlScriptOptions sqlScriptOptions,
        IServiceProvider serviceProvider)
            : this(assembly, filter, filter, sqlScriptOptions, serviceProvider)
    {
    }

    public EmbeddedScriptAndCodeWithInjectionProvider(
        Assembly assembly,
        Func<string, bool> filter,
        Func<string, bool> codeScriptFilter,
        SqlScriptOptions sqlScriptOptions,
        IServiceProvider serviceProvider)
    {
        _assembly = assembly;
        _filter = codeScriptFilter ?? filter;
        _sqlScriptOptions = sqlScriptOptions;
        _embeddedScriptProvider = new EmbeddedScriptProvider(assembly, filter, DbUpDefaults.DefaultEncoding, sqlScriptOptions);
        _serviceProvider = serviceProvider;
    }

    public IEnumerable<SqlScript> GetScripts(IConnectionManager connectionManager) =>
        _embeddedScriptProvider
            .GetScripts(connectionManager)
            .Concat(GetScriptsFromScriptClasses(connectionManager))
            .OrderBy(s => s.Name)
            .Where(s => _filter(s.Name));

    private IEnumerable<SqlScript> GetScriptsFromScriptClasses(IConnectionManager connectionManager)
    {
        const string CsExtension = ".cs";
        var scriptType = typeof(IScript);

        return connectionManager.ExecuteCommandsWithManagedConnection(dbCommandFactory =>
            _assembly
                .GetTypes()
                .Where(t => scriptType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                .Select(t =>
                {
                    var name = $"{t.FullName}{CsExtension}";
                    var constructor = t.GetConstructors().FirstOrDefault();
                    object? instance = null;

                    if (constructor is not null)
                    {
                        var parameters = constructor.GetParameters();
                        var resolvedParameters = new object[parameters.Length];

                        for (int i = 0; i < parameters.Length; i++)
                        {
                            var resolvedParameter = _serviceProvider.GetRequiredService(parameters[i].ParameterType);
                            resolvedParameters[i] = resolvedParameter;
                        }

                        instance = Activator.CreateInstance(t, resolvedParameters);
                    }
                    else
                    {
                        instance = Activator.CreateInstance(t);
                    }

                    return new LazySqlScript(
                        name,
                        _sqlScriptOptions,
                        () => ((IScript)instance!).ProvideScript(dbCommandFactory));
                }));
    }
}
