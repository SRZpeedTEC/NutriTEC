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

    public AuthController(
        IAuthService authService,
        IValidator<RegisterClientRequest> registerClientValidator)
    {
        _authService = authService;
        _registerClientValidator = registerClientValidator;
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
            var errors = validationResult.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(error => error.ErrorMessage).ToArray());

            return BadRequest(new ValidationProblemDetails(errors)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Se encontraron uno o mas errores de validacion."
            });
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
}
