namespace NutriTEC.Domain.Entities;

public class MealTime
{
    // Meal-time rows provide the scope that separates plan and daily product collections.
    public int MealTimeId { get; set; }

    public string MealType { get; set; } = string.Empty;

    public ICollection<MealTimeProduct> Products { get; set; } = new List<MealTimeProduct>();

    public ICollection<DailyMealTime> DailyMealTimes { get; set; } = new List<DailyMealTime>();

    public ICollection<PlanMealTime> PlanMealTimes { get; set; } = new List<PlanMealTime>();
}
