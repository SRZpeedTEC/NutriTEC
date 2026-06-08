using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NutriTEC.Application.DTOs.Auth;
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
        var validationResult = await _registerClientValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return BadRequest(CreateValidationProblemDetails(validationResult));
        }

        var result = await _authService.RegisterClientAsync(request, cancellationToken);

        if (result.Conflict)
        {
            return Conflict(new
            {
                message = result.ErrorMessage
            });
        }

        return Created("/api/auth/register/client", result.Value);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        // The controller validates request shape and delegates credential checks to the service.
        var validationResult = await _loginValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return BadRequest(CreateValidationProblemDetails(validationResult));
        }

        var result = await _authService.LoginAsync(request, cancellationToken);

        if (result.Unauthorized)
        {
            return Unauthorized(new
            {
                message = result.ErrorMessage
            });
        }

        return Ok(result.Value);
    }

    private static ValidationProblemDetails CreateValidationProblemDetails(FluentValidation.Results.ValidationResult validationResult)
    {
        // Validation responses use the same shape for registration and login endpoints.
        var errors = validationResult.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.ErrorMessage).ToArray());

        return new ValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Se encontraron uno o mas errores de validacion."
        };
    }
}
