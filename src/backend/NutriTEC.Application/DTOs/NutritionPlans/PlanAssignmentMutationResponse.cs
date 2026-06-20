namespace NutriTEC.Application.DTOs.NutritionPlans;

public class PlanAssignmentMutationResponse
{
    public int AssignmentId { get; set; }

    public int PlanId { get; set; }

    public int ClientId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string AssignmentStatus { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}
