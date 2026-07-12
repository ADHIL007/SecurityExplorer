using System.Text.Json;
using Microsoft.AspNetCore.Http;
using SecurityExplorer.Core;

namespace SecurityExplorer.UI;

public class SecurityExplorerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _url;

    public SecurityExplorerMiddleware(RequestDelegate next, string url)
    {
        _next = next;
        _url = url.StartsWith('/') ? url : $"/{url}";
    }

    public async Task InvokeAsync(HttpContext context, SecurityTestRunner testRunner)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        var isRoot = path.Equals(_url, StringComparison.OrdinalIgnoreCase) ||
                     path.Equals(_url + "/", StringComparison.OrdinalIgnoreCase) ||
                     path.Equals(_url + "/index.html", StringComparison.OrdinalIgnoreCase);

        var isRun = path.Equals(_url + "/run", StringComparison.OrdinalIgnoreCase) ||
                    path.Equals(_url + "/api/run", StringComparison.OrdinalIgnoreCase);

        if (isRoot)
        {
            var assembly = typeof(SecurityExplorerMiddleware).Assembly;
            using var stream = assembly.GetManifestResourceStream("SecurityExplorer.UI.wwwroot.index.html");
            if (stream == null)
            {
                context.Response.StatusCode = 404;
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync("Embedded index.html not found.");
                return;
            }

            context.Response.ContentType = "text/html; charset=utf-8";
            await stream.CopyToAsync(context.Response.Body);
            return;
        }

        if (isRun)
        {
            var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";
            var results = await testRunner.ExecuteAllTestsAsync(baseUrl);

            context.Response.ContentType = "application/json";
            var json = JsonSerializer.Serialize(results);
            await context.Response.WriteAsync(json);
            return;
        }

        await _next(context);
    }
}
