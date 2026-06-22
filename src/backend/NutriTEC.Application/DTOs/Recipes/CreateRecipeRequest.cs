namespace NutriTEC.Application.DTOs.Recipes;

public class CreateRecipeRequest
{
    // Creation carries ownership, a display name, and the complete initial ingredient set.
    public int ClientId { get; set; }

    public string RecipeName { get; set; } = string.Empty;

    public List<RecipeIngredientRequest> Products { get; set; } = new();
}
