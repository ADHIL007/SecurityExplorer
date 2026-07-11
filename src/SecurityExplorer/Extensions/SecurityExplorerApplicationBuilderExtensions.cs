using Microsoft.Extensions.DependencyInjection;
using SecurityExplorer.Abstractions;
using SecurityExplorer.Core;

namespace SecurityExplorer.Extensions;

public static class SecurityExplorerApplicationBuilderExtensions
{

    public static IServiceCollection AddSecurityExplorer(this IServiceCollection services)
    {
        services.Configure<SecurityExplorerOptions>(options => { });
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var testTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IsecurityTest).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var type in testTypes)
        {
            services.AddTransient(typeof(IsecurityTest), type);
        }

        services.AddScoped<SecurityTestRunner>();
        return services;
    }
}

