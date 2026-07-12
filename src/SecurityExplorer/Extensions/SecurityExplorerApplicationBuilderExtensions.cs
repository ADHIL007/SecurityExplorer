using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SecurityExplorer.Abstractions;
using SecurityExplorer.Core;
using SecurityExplorer.UI;

namespace SecurityExplorer.Extensions;

public static class SecurityExplorerApplicationBuilderExtensions
{

    public static IApplicationBuilder UseSecurityExplorer(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices.GetRequiredService<IOptions<SecurityExplorerOptions>>().Value;

        return app.UseMiddleware<SecurityExplorerMiddleware>(options.RoutePrefix);

    }
}

