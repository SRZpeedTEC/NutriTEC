using Microsoft.EntityFrameworkCore;
using NutriTEC.Application.Interfaces.Products;
using NutriTEC.Domain.Entities;
using NutriTEC.Domain.Enums;
using NutriTEC.Infrastructure.Persistence;

namespace NutriTEC.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly NutriTecDbContext _dbContext;

    public ProductRepository(NutriTecDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> BarCodeExistsAsync(string barCode, CancellationToken cancellationToken)
    {
        // Barcode checks use the product primary key.
        return _dbContext.Products.AnyAsync(product => product.BarCode == barCode, cancellationToken);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        // Product creation is tracked by EF until the service commits the unit of work.
        await _dbContext.Products.AddAsync(product, cancellationToken);
    }

    public Task<Product?> GetByBarCodeAsync(string barCode, CancellationToken cancellationToken)
    {
        // Edit and delete need a tracked entity so service changes can be saved.
        return _dbContext.Products.FirstOrDefaultAsync(product => product.BarCode == barCode, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Product>> GetByBarCodesAsync(
        IReadOnlyCollection<string> barCodes,
        CancellationToken cancellationToken)
    {
        // Batch lookup lets recipe workflows validate every ingredient with one database query.
        return await _dbContext.Products
            .AsNoTracking()
            .Where(product => barCodes.Contains(product.BarCode))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyDictionary<string, string>> GetNamesByBarCodesAsync(
        IReadOnlyCollection<string> barCodes,
        CancellationToken cancellationToken)
    {
        // Projection to two varchar columns avoids EF type-mapping issues with the full Product entity.
        var rows = await _dbContext.Products
            .AsNoTracking()
            .Where(p => barCodes.Contains(p.BarCode))
            .Select(p => new { p.BarCode, p.ProductName })
            .ToListAsync(cancellationToken);

        return (IReadOnlyDictionary<string, string>)rows.ToDictionary(
            r => r.BarCode,
            r => r.ProductName,
            StringComparer.OrdinalIgnoreCase);
    }

    public async Task<IReadOnlyCollection<Product>> SearchActiveAsync(
        string query,
        CancellationToken cancellationToken)
    {
        // Selection searches are restricted to active products and match either user-facing identifier.
        return await _dbContext.Products
            .AsNoTracking()
            .Where(product => product.ProductStatus == ProductStatus.Active)
            .Where(product => product.ProductName.Contains(query) || product.BarCode.Contains(query))
            .OrderBy(product => product.ProductName)
            .ThenBy(product => product.BarCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Product>> GetPendingByUserIdAsync(
        int userId,
        CancellationToken cancellationToken)
    {
        // Pending product lists are read-only and scoped by submitter.
        return await _dbContext.Products
            .AsNoTracking()
            .Where(product => product.UserId == userId)
            .Where(product => product.ProductStatus == ProductStatus.PendingReview)
            .OrderBy(product => product.ProductName)
            .ToListAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public void Delete(Product product)
    {
        // The current project has no soft-delete convention, so pending products are removed directly.
        _dbContext.Products.Remove(product);
    }
}
