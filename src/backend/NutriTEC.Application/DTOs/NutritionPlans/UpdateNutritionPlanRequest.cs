namespace NutriTEC.Application.DTOs.NutritionPlans;

public class UpdateNutritionPlanRequest
{
    public string PlanName { get; set; } = string.Empty;

    public int NutritionistCode { get; set; }

    public List<PlanMealTimeRequest> MealTimes { get; set; } = new();
}
