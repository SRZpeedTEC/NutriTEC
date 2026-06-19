using NutriTEC.Domain.Entities;

namespace NutriTEC.Application.Interfaces.Products;

public interface IProductRepository
{
    // Barcode uniqueness is checked against persisted product identifiers.
    Task<bool> BarCodeExistsAsync(string barCode, CancellationToken cancellationToken);

    Task AddAsync(Product product, CancellationToken cancellationToken);

    Task<Product?> GetByBarCodeAsync(string barCode, CancellationToken cancellationToken);

    // Daily consumption searches expose only active products matching name or barcode.
    Task<IReadOnlyCollection<Product>> SearchActiveAsync(string query, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Product>> GetPendingByUserIdAsync(int userId, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);

    void Delete(Product product);
}
