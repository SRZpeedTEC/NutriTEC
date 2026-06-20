namespace NutriTEC.Application.DTOs.NutritionPlans;

public class PlanMealTimeProductRequest
{
    public string ProductCode { get; set; } = string.Empty;

    public decimal Quantity { get; set; }
}
