namespace NutriTEC.Application.DTOs.DailyConsume;

public class DailyConsumeTodayResponse
{
    // Daily home combines the trigger-maintained total, client goal, and grouped product details.
    public int ClientId { get; set; }

    public DateOnly ConsumeDate { get; set; }

    public decimal TotalCalories { get; set; }

    public decimal MaxDailyCalories { get; set; }

    public decimal RemainingCalories { get; set; }

    public IReadOnlyCollection<DailyMealTimeResponse> MealTimes { get; set; } = Array.Empty<DailyMealTimeResponse>();
}
