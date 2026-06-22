namespace NutriTEC.Domain.Entities;

public class PlanMealTime
{
    public int PlanMealTimeId { get; set; }

    public int MealTimeId { get; set; }

    public int PlanId { get; set; }

    public NutritionPlan Plan { get; set; } = null!;

    public MealTime MealTime { get; set; } = null!;
}
