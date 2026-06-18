namespace NutriTEC.Application.DTOs.Auth;

public class ActivePlanSummaryResponse
{
    // The active plan summary mirrors fields available in plan_assignment and nutrition_plan.
    public int AssignmentId { get; set; }

    public int PlanId { get; set; }

    public string PlanName { get; set; } = string.Empty;

    public decimal TotalCalories { get; set; }

    public int NutritionistCode { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string AssignmentStatus { get; set; } = string.Empty;
}
