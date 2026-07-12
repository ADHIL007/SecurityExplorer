
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SecurityExplorer.Abstractions;
using SecurityExplorer.Core;
using SecurityExplorer.UI;

namespace SecurityExplorer.Extensions;

public static class SecurityExplorerServiceCollectionExtensions
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

