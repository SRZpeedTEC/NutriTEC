namespace NutriTEC.Domain.Entities;

public class DailyMealTime
{
    // This link assigns one meal-time product collection to a client's consumption date.
    public int DailyMealTimeId { get; set; }

    public int ClientId { get; set; }

    public DateOnly ConsumeDate { get; set; }

    public int MealTimeId { get; set; }

    public DailyConsume DailyConsume { get; set; } = null!;

    public MealTime MealTime { get; set; } = null!;
}
