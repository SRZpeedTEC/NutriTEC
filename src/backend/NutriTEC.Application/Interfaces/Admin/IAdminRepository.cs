using NutriTEC.Application.DTOs.Admin;

namespace NutriTEC.Application.Interfaces.Admin;

public interface IAdminRepository
{
    // Admin reads use the stored procedures defined for the administrator view.
    Task<AdminLoginRecord?> GetAdminLoginByEmailAsync(
        string email,
        CancellationToken cancellationToken);

    Task<bool> AdminExistsAsync(int adminId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<AdminProductResponse>> GetProductsAsync(
        string? productStatus,
        CancellationToken cancellationToken);

    Task<AdminProductResponse?> GetProductByBarCodeAsync(
        string barCode,
        CancellationToken cancellationToken);

    Task<AdminProductResponse?> UpdateProductStatusByAdminAsync(
        string barCode,
        int adminId,
        string newStatus,
        CancellationToken cancellationToken);
}
