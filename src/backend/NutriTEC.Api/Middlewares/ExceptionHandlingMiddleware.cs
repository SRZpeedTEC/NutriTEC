using System.Net;
using NutriTEC.Application.Exceptions;

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
        catch (ApplicationValidationException exception)
        {
            await WriteValidationErrorAsync(context, exception);
        }
        catch (ConflictException exception)
        {
            await WriteApplicationErrorAsync(context, HttpStatusCode.Conflict, exception.Message);
        }
        catch (NutriTEC.Application.Exceptions.UnauthorizedException exception)
        {
            await WriteApplicationErrorAsync(context, HttpStatusCode.Unauthorized, exception.Message);
        }
        catch (NutriTecApplicationException exception)
        {
            await WriteApplicationErrorAsync(context, HttpStatusCode.BadRequest, exception.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An unexpected error occurred while processing the request.");
            await WriteUnexpectedErrorAsync(context);
        }
    }

    private static async Task WriteValidationErrorAsync(
        HttpContext context,
        ApplicationValidationException exception)
    {
        // Known validation failures are returned with field errors while keeping one API response shape.
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        await context.Response.WriteAsJsonAsync(new
        {
            statusCode = context.Response.StatusCode,
            message = exception.Message,
            errors = exception.Errors
        });
    }

    private static async Task WriteApplicationErrorAsync(
        HttpContext context,
        HttpStatusCode statusCode,
        string message)
    {
        // Known application failures are expected outcomes and do not expose infrastructure details.
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        await context.Response.WriteAsJsonAsync(new
        {
            statusCode = context.Response.StatusCode,
            message
        });
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
