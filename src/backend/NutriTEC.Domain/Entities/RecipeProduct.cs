namespace NutriTEC.Domain.Entities;

public class RecipeProduct
{
    // The composite key identifies one product and its portion quantity inside a recipe.
    public int RecipeId { get; set; }

    public string ProductCode { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public Recipe Recipe { get; set; } = null!;

    public Product Product { get; set; } = null!;
}
