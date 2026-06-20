namespace NutriTEC.Application.DTOs.Recipes;

public class RecipeDetailResponse
{
    // Detail responses combine stored recipe totals with all calculated ingredient contributions.
    public int RecipeId { get; set; }

    public int ClientId { get; set; }

    public string RecipeName { get; set; } = string.Empty;

    public decimal TotalCalories { get; set; }

    public RecipeNutritionTotalsResponse NutritionTotals { get; set; } = new();

    public IReadOnlyCollection<RecipeIngredientResponse> Products { get; set; } = Array.Empty<RecipeIngredientResponse>();
}
