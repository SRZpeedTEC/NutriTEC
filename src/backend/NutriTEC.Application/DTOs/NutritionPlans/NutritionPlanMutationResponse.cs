namespace NutriTEC.Application.DTOs.NutritionPlans;

public class NutritionPlanMutationResponse
{
    public string Message { get; set; } = string.Empty;

    public NutritionPlanDetailResponse Plan { get; set; } = null!;
}
