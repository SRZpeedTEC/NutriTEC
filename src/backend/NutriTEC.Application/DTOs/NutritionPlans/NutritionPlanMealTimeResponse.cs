namespace NutriTEC.Application.DTOs.NutritionPlans;

public class NutritionPlanMealTimeResponse
{
    public int MealTimeId { get; set; }

    public string MealType { get; set; } = string.Empty;

    public decimal TotalCalories { get; set; }

    public IReadOnlyCollection<NutritionPlanMealTimeProductResponse> Products { get; set; } = [];
}
