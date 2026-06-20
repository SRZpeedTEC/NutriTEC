namespace NutriTEC.Application.DTOs.NutritionPlans;

public class NutritionPlanMealTimeProductResponse
{
    public string ProductCode { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public decimal Calories { get; set; }
}
