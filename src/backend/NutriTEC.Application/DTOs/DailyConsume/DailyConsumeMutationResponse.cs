namespace NutriTEC.Application.DTOs.DailyConsume;

public class DailyConsumeMutationResponse
{
    // Mutations return the affected meal time and fresh total after the SQL trigger runs.
    public string Message { get; set; } = string.Empty;

    public decimal TotalDailyCalories { get; set; }

    public DailyMealTimeResponse MealTime { get; set; } = new();
}
