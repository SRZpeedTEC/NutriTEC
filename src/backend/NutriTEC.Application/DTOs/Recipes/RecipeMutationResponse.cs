namespace NutriTEC.Application.DTOs.Recipes;

public class RecipeMutationResponse
{
    // Create and update share one confirmation shape with a fresh recipe snapshot.
    public string Message { get; set; } = string.Empty;

    public RecipeDetailResponse Recipe { get; set; } = new();
}
