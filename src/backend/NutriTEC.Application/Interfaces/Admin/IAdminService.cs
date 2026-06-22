using NutriTEC.Application.DTOs.Admin;
using NutriTEC.Application.DTOs.Auth;

namespace NutriTEC.Application.Interfaces.Admin;

public interface IAdminService
{
    Task<AdminLoginResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<AdminProductResponse>> GetProductsAsync(
        string? productStatus,
        CancellationToken cancellationToken);

    Task<AdminBillingReportResponse> GetBillingReportAsync(
        AdminBillingReportRequest request,
        CancellationToken cancellationToken);

    Task<AdminProductResponse> GetProductByBarCodeAsync(
        string barCode,
        CancellationToken cancellationToken);

    Task<AdminProductMutationResponse> ApproveProductAsync(
        string barCode,
        int adminId,
        CancellationToken cancellationToken);

    Task<AdminProductMutationResponse> RejectProductAsync(
        string barCode,
        int adminId,
        CancellationToken cancellationToken);
}
