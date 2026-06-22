namespace NutriTEC.Application.DTOs.NutritionPlans;

public class AssignPlanRequest
{
    public int ClientId { get; set; }

    public int NutritionistCode { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }
}
