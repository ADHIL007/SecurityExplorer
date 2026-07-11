
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SecurityExplorer.Core;
using SecurityExplorer.UI;

namespace SecurityExplorer.Extensions;

public static class SecurityExplorerServiceCollectionExtensions
{

    public static IApplicationBuilder UseSecurityExplorer(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices.GetRequiredService<IOptions<SecurityExplorerOptions>>().Value;

        return app.UseMiddleware<SecurityExplorerMiddleware>(options.RoutePrefix);

    }

}

