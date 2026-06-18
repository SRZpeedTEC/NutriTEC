namespace NutriTEC.Domain.Entities;

public class PlanAssignment
{
    // Plan assignments determine whether a client has an active plan for the current date.
    public int AssignmentId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string AssignmentStatus { get; set; } = string.Empty;

    public int PlanId { get; set; }

    public NutritionPlan NutritionPlan { get; set; } = null!;

    public int ClientId { get; set; }

    public Client Client { get; set; } = null!;
}
