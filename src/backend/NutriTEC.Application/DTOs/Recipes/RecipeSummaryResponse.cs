namespace NutriTEC.Application.DTOs.Recipes;

public class RecipeSummaryResponse
{
    // Recipe lists use a compact shape suitable for client selection screens.
    public int RecipeId { get; set; }

    public string RecipeName { get; set; } = string.Empty;

    public decimal TotalCalories { get; set; }

    public int IngredientCount { get; set; }
}
