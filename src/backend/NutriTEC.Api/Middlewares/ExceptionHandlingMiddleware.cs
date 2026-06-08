using System.Net;

namespace NutriTEC.Api.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // The next middleware may throw unexpected errors that should be converted into one JSON response shape.
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An unexpected error occurred while processing the request.");
            await WriteUnexpectedErrorAsync(context);
        }
    }

    private static async Task WriteUnexpectedErrorAsync(HttpContext context)
    {
        // Production-style error responses avoid leaking stack traces or internal exception messages to clients.
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        await context.Response.WriteAsJsonAsync(new
        {
            statusCode = context.Response.StatusCode,
            message = "Ocurrio un error inesperado. Intente nuevamente mas tarde."
        });
    }
}
