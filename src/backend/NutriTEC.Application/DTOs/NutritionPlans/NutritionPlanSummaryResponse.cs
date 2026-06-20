namespace NutriTEC.Application.DTOs.NutritionPlans;

public class NutritionPlanSummaryResponse
{
    public int PlanId { get; set; }

    public string PlanName { get; set; } = string.Empty;

    public decimal TotalCalories { get; set; }

    public int NutritionistCode { get; set; }

    public int MealTimeCount { get; set; }
}
