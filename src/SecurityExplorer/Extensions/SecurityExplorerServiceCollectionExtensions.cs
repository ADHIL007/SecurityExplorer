
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection;
using SecurityExplorer.Abstractions;
using SecurityExplorer.Core;

namespace SecurityExplorer.Extensions;

public static class SecurityExplorerServiceCollectionExtensions
{
    public static IServiceCollection AddSecurityExplorer(this IServiceCollection services, Action<SecurityExplorerOptions>? configureOptions = null)
    {
        var options = new SecurityExplorerOptions();
        configureOptions?.Invoke(options);
        services.Configure<SecurityExplorerOptions>(o => {
            if (configureOptions != null)
            {
                configureOptions(o);
            }
        });

        // Resolve absolute plugins path
        var pluginsPath = Path.IsPathRooted(options.PluginsPath)
            ? options.PluginsPath
            : Path.Combine(AppContext.BaseDirectory, options.PluginsPath);

        // Scan folder for DLL files and dynamically load them
        if (Directory.Exists(pluginsPath))
        {
            var dllFiles = Directory.GetFiles(pluginsPath, "*.dll");
            foreach (var dllFile in dllFiles)
            {
                try
                {
                    AssemblyLoadContext.Default.LoadFromAssemblyPath(dllFile);
                }
                catch (Exception ex)
                {
                    // Ignore non-.NET assemblies or files that cannot be loaded
                    Console.WriteLine($"[SecurityExplorer] Could not load plugin assembly '{dllFile}': {ex.Message}");
                }
            }
        }

        // Scan all loaded assemblies for types implementing ISecurityTest
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var testTypes = assemblies
            .SelectMany(a => {
                try { return a.GetTypes(); }
                catch { return Type.EmptyTypes; }
            })
            .Where(t => typeof(ISecurityTest).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var type in testTypes)
        {
            services.AddTransient(typeof(ISecurityTest), type);
        }

        services.AddScoped<SecurityTestRunner>();
        return services;
    }
}

