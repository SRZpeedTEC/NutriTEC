using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NutriTEC.Application.DTOs.Auth;
using NutriTEC.Application.Exceptions;
using NutriTEC.Application.Interfaces.Auth;

namespace NutriTEC.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IValidator<RegisterClientRequest> _registerClientValidator;
    private readonly IValidator<LoginRequest> _loginValidator;

    public AuthController(
        IAuthService authService,
        IValidator<RegisterClientRequest> registerClientValidator,
        IValidator<LoginRequest> loginValidator)
    {
        _authService = authService;
        _registerClientValidator = registerClientValidator;
        _loginValidator = loginValidator;
    }

    [HttpPost("register/client")]
    public async Task<IActionResult> RegisterClient(
        RegisterClientRequest request,
        CancellationToken cancellationToken)
    {
        // The controller performs request validation and delegates registration decisions to the service.
        await ValidateRequestAsync(_registerClientValidator, request, cancellationToken);

        var response = await _authService.RegisterClientAsync(request, cancellationToken);
        return Created("/api/auth/register/client", response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        // The controller validates request shape and delegates credential checks to the service.
        await ValidateRequestAsync(_loginValidator, request, cancellationToken);

        var response = await _authService.LoginAsync(request, cancellationToken);
        return Ok(response);
    }

    private static async Task ValidateRequestAsync<TRequest>(
        IValidator<TRequest> validator,
        TRequest request,
        CancellationToken cancellationToken)
    {
        // Controllers keep validation invocation local while middleware owns the HTTP error response.
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ApplicationValidationException(CreateValidationErrors(validationResult));
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
