namespace NutriTEC.Application.DTOs.DailyConsume;

public class AddDailyProductRequest
{
    // The request identifies the client, selected meal type, approved product, and consumed portions.
    public int ClientId { get; set; }

    // El tiempo de comida se elige por tipo (BREAKFAST, LUNCH, ...); el backend crea su registro del dia.
    public string MealType { get; set; } = string.Empty;

    public string ProductCode { get; set; } = string.Empty;

    public decimal Quantity { get; set; }
}
