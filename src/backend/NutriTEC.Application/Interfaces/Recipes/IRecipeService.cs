using NutriTEC.Application.DTOs.DailyConsume;
using NutriTEC.Application.DTOs.Recipes;

namespace NutriTEC.Application.Interfaces.Recipes;

public interface IRecipeService
{
    // Recipe workflows manage client-owned templates and their transactional daily expansion.
    Task<RecipeMutationResponse> CreateAsync(
        CreateRecipeRequest request,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<RecipeSummaryResponse>> GetByClientAsync(
        int clientId,
        CancellationToken cancellationToken);

    Task<RecipeDetailResponse> GetDetailAsync(
        int recipeId,
        int clientId,
        CancellationToken cancellationToken);

    Task<RecipeMutationResponse> UpdateAsync(
        int recipeId,
        UpdateRecipeRequest request,
        CancellationToken cancellationToken);

    Task<RecipeDeletedResponse> DeleteAsync(
        int recipeId,
        int clientId,
        CancellationToken cancellationToken);

    Task<DailyConsumeMutationResponse> AddToDailyConsumeAsync(
        int recipeId,
        AddRecipeToDailyConsumeRequest request,
        CancellationToken cancellationToken);
}
