using System.Net;
using System.Text.Json;
using Serilog;

namespace SecurityExplorer.SampleWebApi.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                Log.Information(
                    "Processing Request: {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);

                await _next(context);

                Log.Information(
                    "Request Completed: {Method} {Path} - Status Code: {StatusCode}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode);
            }
            catch (Exception ex)
            {
                Log.Error(
                    ex,
                    "Unhandled Exception while processing {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);

                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            context.Response.StatusCode = exception switch
            {
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                ArgumentException => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = context.Response.StatusCode == 500
                    ? "An unexpected error occurred."
                    : exception.Message
            };

            Log.Warning(
                "Returning Error Response: {StatusCode} - {Message}",
                context.Response.StatusCode,
                response.Message);

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    }
}
