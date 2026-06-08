using NutriTEC.Application.DTOs.Auth;

namespace NutriTEC.Application.Interfaces.Auth;

public interface IAuthService
{
    // Registration returns a typed result so controllers can translate outcomes into HTTP responses.
    Task<AuthServiceResult<RegisterClientResponse>> RegisterClientAsync(
        RegisterClientRequest request,
        CancellationToken cancellationToken);

    // Login returns session data for supported account subtypes without exposing credential details.
    Task<AuthServiceResult<LoginResponse>> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken);
}
