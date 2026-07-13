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

        // Redirect exact match without trailing slash to ensure relative paths work
        if (path.Equals(_url, StringComparison.OrdinalIgnoreCase))
        {
            context.Response.Redirect(_url + "/");
            return;
        }

        // 1. API Route: Execute Tests
        var isRun = path.Equals(_url + "/run", StringComparison.OrdinalIgnoreCase) ||
                    path.Equals(_url + "/api/run", StringComparison.OrdinalIgnoreCase);

        if (isRun)
        {
            var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";
            var results = await testRunner.ExecuteAllTestsAsync(baseUrl);

            context.Response.ContentType = "application/json";
            var json = JsonSerializer.Serialize(results);
            await context.Response.WriteAsync(json);
            return;
        }

        // 2. Serve Embedded Static UI Files
        if (path.StartsWith(_url, StringComparison.OrdinalIgnoreCase))
        {
            var relativePath = path.Substring(_url.Length).TrimStart('/');
            if (string.IsNullOrEmpty(relativePath))
            {
                relativePath = "index.html"; // Default document
            }

            // Convert path to embedded resource name (e.g., "styles.css" -> "SecurityExplorer.UI.wwwroot.styles.css")
            var resourceName = $"SecurityExplorer.UI.wwwroot.{relativePath.Replace("/", ".")}";
            var assembly = typeof(SecurityExplorerMiddleware).Assembly;
            
            // Perform case-insensitive search to handle Windows/Browser casing discrepancies
            var actualResourceName = assembly.GetManifestResourceNames()
                .FirstOrDefault(name => name.Equals(resourceName, StringComparison.OrdinalIgnoreCase));

            if (actualResourceName != null)
            {
                using var stream = assembly.GetManifestResourceStream(actualResourceName);
                if (stream != null)
                {
                    var ext = Path.GetExtension(relativePath).ToLowerInvariant();
                    context.Response.ContentType = ext switch
                    {
                        ".html" => "text/html; charset=utf-8",
                        ".css" => "text/css; charset=utf-8",
                        ".js" => "application/javascript; charset=utf-8",
                        _ => "text/plain"
                    };
                    
                    await stream.CopyToAsync(context.Response.Body);
                    return;
                }
            }
            
            if (relativePath.Equals("index.html", StringComparison.OrdinalIgnoreCase))
            {
                 // Fallback error if the main shell is truly missing
                 context.Response.StatusCode = 404;
                 context.Response.ContentType = "text/plain";
                 await context.Response.WriteAsync("Embedded index.html not found. Ensure it is set as an EmbeddedResource.");
                 return;
            }
        }

        await _next(context);
    }
}
