namespace NutriTEC.Application.DTOs.Recipes;

public class AddRecipeToDailyConsumeRequest
{
    // The multiplier scales every recipe ingredient before daily-consumption expansion.
    public int ClientId { get; set; }

    // El tiempo de comida se elige por tipo (BREAKFAST, LUNCH, ...); el backend crea su registro del dia.
    public string MealType { get; set; } = string.Empty;

    public decimal Multiplier { get; set; } = 1;
}
