namespace NutriTEC.Application.DTOs.Recipes;

public class RecipeNutritionTotalsResponse
{
    // Aggregate nutrition values are calculated from vw_recipe_product_detail rows rather than stored redundantly.
    public decimal Calories { get; set; }

    public decimal Fat { get; set; }

    public decimal Sodium { get; set; }

    public decimal Carbohydrates { get; set; }

    public decimal Protein { get; set; }

    public string Vitamins { get; set; } = string.Empty;

    public decimal Calcium { get; set; }

    public decimal Iron { get; set; }
}
