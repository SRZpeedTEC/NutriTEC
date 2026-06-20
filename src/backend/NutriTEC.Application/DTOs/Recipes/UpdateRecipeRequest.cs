namespace NutriTEC.Application.DTOs.Recipes;

public class UpdateRecipeRequest
{
    // Updates replace the recipe name and complete ingredient set for future uses only.
    public int ClientId { get; set; }

    public string RecipeName { get; set; } = string.Empty;

    public List<RecipeIngredientRequest> Products { get; set; } = new();
}
