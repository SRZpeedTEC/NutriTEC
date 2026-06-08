using NutriTEC.Application.DTOs.Auth;

namespace NutriTEC.Application.Interfaces.Auth;

public interface IAuthService
{
    // Registration returns a typed result so controllers can translate outcomes into HTTP responses.
    Task<AuthServiceResult<RegisterClientResponse>> RegisterClientAsync(
        RegisterClientRequest request,
        CancellationToken cancellationToken);
}
