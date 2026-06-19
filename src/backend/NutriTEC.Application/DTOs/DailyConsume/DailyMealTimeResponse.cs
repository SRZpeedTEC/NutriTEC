namespace NutriTEC.Application.DTOs.DailyConsume;

public class DailyMealTimeResponse
{
    // The meal-time identifier and product barcode form the current editable detail identity.
    public int MealTimeId { get; set; }

    public string MealType { get; set; } = string.Empty;

    public decimal TotalCalories { get; set; }

    public IReadOnlyCollection<DailyConsumedProductResponse> Products { get; set; } = Array.Empty<DailyConsumedProductResponse>();
}
