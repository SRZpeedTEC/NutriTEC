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
        await _registerClientValidator.ValidateRequestAsync(request, cancellationToken);

        var response = await _authService.RegisterClientAsync(request, cancellationToken);
        return Created("/api/auth/register/client", response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        // The controller validates request shape and delegates credential checks to the service.
        await _loginValidator.ValidateRequestAsync(request, cancellationToken);

        var response = await _authService.LoginAsync(request, cancellationToken);
        return Ok(response);
    }
}
