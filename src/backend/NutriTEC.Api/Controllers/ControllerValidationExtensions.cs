using FluentValidation;
using NutriTEC.Application.Exceptions;

namespace NutriTEC.Api.Controllers;

internal static class ControllerValidationExtensions
{
    public static async Task ValidateRequestAsync<TRequest>(
        this IValidator<TRequest> validator,
        TRequest request,
        CancellationToken cancellationToken)
    {
        // Controllers invoke validators while middleware owns the HTTP error response.
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ApplicationValidationException(CreateValidationErrors(validationResult));
        }
    }

    public static void ValidateRequiredRouteValue(
        string propertyName,
        string? value,
        string message)
    {
        // Route values are validated here so controllers do not build HTTP error responses directly.
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ApplicationValidationException(new Dictionary<string, string[]>
            {
                [propertyName] = new[] { message }
            });
        }
    }

    public static void ValidatePositiveRouteValue(
        string propertyName,
        int value,
        string message)
    {
        // Numeric route values are validated consistently before reaching services.
        if (value <= 0)
        {
            throw new ApplicationValidationException(new Dictionary<string, string[]>
            {
                [propertyName] = new[] { message }
            });
        }
    }

    private static IReadOnlyDictionary<string, string[]> CreateValidationErrors(FluentValidation.Results.ValidationResult validationResult)
    {
        // Validation errors preserve field grouping for the global middleware response.
        return validationResult.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.ErrorMessage).ToArray());
    }
}
