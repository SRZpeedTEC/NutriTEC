namespace NutriTEC.Application.DTOs.Recipes;

public class RecipeIngredientRequest
{
    // Each ingredient references an existing product and stores its number of portions.
    public string ProductCode { get; set; } = string.Empty;

    public decimal Quantity { get; set; }
}
