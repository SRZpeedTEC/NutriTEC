namespace NutriTEC.Application.DTOs.NutritionPlans;

public class ClientActivePlanDetailResponse
{
    public int AssignmentId { get; set; }

    public int PlanId { get; set; }

    public string PlanName { get; set; } = string.Empty;

    public decimal TotalCalories { get; set; }

    public int NutritionistCode { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public IReadOnlyCollection<NutritionPlanMealTimeResponse> MealTimes { get; set; } = [];
}
