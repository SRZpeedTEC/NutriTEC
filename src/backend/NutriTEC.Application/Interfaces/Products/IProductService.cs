using NutriTEC.Application.DTOs.Products;

namespace NutriTEC.Application.Interfaces.Products;

public interface IProductService
{
    // Product submission and pending-management operations are reusable by clients and nutritionists.
    Task<ProductMutationResponse> CreateProductAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<ProductResponse>> GetPendingProductsByUserAsync(
        int userId,
        CancellationToken cancellationToken);

    Task<ProductMutationResponse> UpdatePendingProductAsync(
        string barCode,
        UpdateProductRequest request,
        CancellationToken cancellationToken);

    Task<ProductDeletedResponse> DeletePendingProductAsync(
        DeleteProductRequest request,
        CancellationToken cancellationToken);
}
