using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NutriTEC.Application.Interfaces.Recipes;
using NutriTEC.Domain.Entities;
using NutriTEC.Infrastructure.Persistence;

namespace NutriTEC.Infrastructure.Repositories;

public class RecipeRepository : IRecipeRepository
{
    private readonly NutriTecDbContext _dbContext;

    public RecipeRepository(NutriTecDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Recipe?> GetByIdAsync(int recipeId, CancellationToken cancellationToken)
    {
        // Tracked ingredients support ownership checks followed by update, delete, or expansion.
        return _dbContext.Recipes
            .Include(recipe => recipe.Products)
            .FirstOrDefaultAsync(recipe => recipe.RecipeId == recipeId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Recipe>> GetByClientIdAsync(
        int clientId,
        CancellationToken cancellationToken)
    {
        // Client recipe lists need ingredient counts but never track returned aggregates.
        return await _dbContext.Recipes
            .AsNoTracking()
            .Include(recipe => recipe.Products)
            .Where(recipe => recipe.ClientId == clientId)
            .OrderBy(recipe => recipe.RecipeName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<RecipeProductDetail>> GetDetailsAsync(
        int recipeId,
        CancellationToken cancellationToken)
    {
        // Complete detail responses are sourced from the SQL view to centralize nutrition calculations.
        return await _dbContext.RecipeProductDetails
            .AsNoTracking()
            .Where(detail => detail.RecipeId == recipeId)
            .OrderBy(detail => detail.ProductName)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Recipe recipe, CancellationToken cancellationToken)
    {
        // Parent and ingredient inserts share one EF transaction; the trigger updates totals afterward.
        await _dbContext.Recipes.AddAsync(recipe, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ReplaceProductsAsync(
        Recipe recipe,
        IReadOnlyCollection<RecipeProduct> products,
        CancellationToken cancellationToken)
    {
        // Two ordered saves avoid tracked composite-key collisions while one transaction keeps replacement atomic.
        await using IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        _dbContext.RecipeProducts.RemoveRange(recipe.Products);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _dbContext.RecipeProducts.AddRangeAsync(products, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task DeleteAsync(Recipe recipe, CancellationToken cancellationToken)
    {
        // Relationships are deleted explicitly before the recipe because the schema intentionally has no cascade.
        await using IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        _dbContext.RecipeProducts.RemoveRange(recipe.Products);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _dbContext.Recipes.Remove(recipe);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }
}
