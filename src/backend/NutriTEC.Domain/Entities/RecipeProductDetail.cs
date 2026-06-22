namespace NutriTEC.Domain.Entities;

public class RecipeProductDetail
{
    // This keyless read model mirrors vw_recipe_product_detail for complete recipe responses.
    public int RecipeId { get; set; }

    public string RecipeName { get; set; } = string.Empty;

    public decimal TotalCalories { get; set; }

    public int ClientId { get; set; }

    public string BarCode { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public string PortionUnit { get; set; } = string.Empty;

    public decimal PortionSize { get; set; }

    public decimal Quantity { get; set; }

    public decimal CalculatedCalories { get; set; }

    public decimal CalculatedFat { get; set; }

    public decimal CalculatedSodium { get; set; }

    public decimal CalculatedCarbohydrates { get; set; }

    public decimal CalculatedProtein { get; set; }

    public string CalculatedVitamins { get; set; } = string.Empty;

    public decimal CalculatedCalcium { get; set; }

    public decimal CalculatedIron { get; set; }
}
