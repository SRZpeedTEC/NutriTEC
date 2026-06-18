using NutriTEC.Application.DTOs.Auth;

namespace NutriTEC.Application.Interfaces.Auth;

public interface IAuthService
{
    // Registration returns the success DTO and throws application exceptions for expected business failures.
    Task<RegisterClientResponse> RegisterClientAsync(
        RegisterClientRequest request,
        CancellationToken cancellationToken);

    // Login returns session data for supported account subtypes without exposing credential details.
    Task<LoginResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken);
}
