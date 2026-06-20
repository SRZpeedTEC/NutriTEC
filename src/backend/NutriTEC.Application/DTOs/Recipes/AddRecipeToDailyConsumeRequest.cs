namespace NutriTEC.Application.DTOs.Recipes;

public class AddRecipeToDailyConsumeRequest
{
    // The multiplier scales every recipe ingredient before daily-consumption expansion.
    public int ClientId { get; set; }

    public int MealTimeId { get; set; }

    public decimal Multiplier { get; set; } = 1;
}
