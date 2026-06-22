namespace NutriTEC.Application.DTOs.NutritionPlans;

public class PlanMealTimeRequest
{
    public string MealType { get; set; } = string.Empty;

    public List<PlanMealTimeProductRequest> Products { get; set; } = new();
}
