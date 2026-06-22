using NutriTEC.Application.DTOs.Auth;

namespace NutriTEC.Application.Interfaces.Auth;

public interface IAuthService
{
    Task<RegisterClientResponse> RegisterClientAsync(
        RegisterClientRequest request,
        CancellationToken cancellationToken);

    Task<RegisterNutritionistResponse> RegisterNutritionistAsync(
        RegisterNutritionistRequest request,
        CancellationToken cancellationToken);

    Task<LoginResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken);
}
