using NutriTEC.Domain.Entities;

namespace NutriTEC.Application.Interfaces.Recipes;

public interface IRecipeRepository
{
    // Recipe reads preserve ownership and expose either tracked aggregates or read-only view rows.
    Task<Recipe?> GetByIdAsync(int recipeId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Recipe>> GetByClientIdAsync(int clientId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<RecipeProductDetail>> GetDetailsAsync(
        int recipeId,
        CancellationToken cancellationToken);

    Task AddAsync(Recipe recipe, CancellationToken cancellationToken);

    Task ReplaceProductsAsync(
        Recipe recipe,
        IReadOnlyCollection<RecipeProduct> products,
        CancellationToken cancellationToken);

    Task DeleteAsync(Recipe recipe, CancellationToken cancellationToken);
}
