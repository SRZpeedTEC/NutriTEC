namespace NutriTEC.Application.DTOs.NutritionPlans;

public class NutritionPlanDetailResponse
{
    public int PlanId { get; set; }

    public string PlanName { get; set; } = string.Empty;

    public decimal TotalCalories { get; set; }

    public int NutritionistCode { get; set; }

    public IReadOnlyCollection<NutritionPlanMealTimeResponse> MealTimes { get; set; } = [];
}
