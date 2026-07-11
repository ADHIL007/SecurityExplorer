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
        if (context.Request.Path.StartsWithSegments(_url))
        {
            var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";
            var results = testRunner.ExecuteAllTestsAsync(baseUrl);

            context.Response.ContentType = "application/json";
            var json = JsonSerializer.Serialize(results);
            await context.Response.WriteAsync(json);
            return;
        }
        await _next(context);
    }


}
