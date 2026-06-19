namespace NutriTEC.Application.DTOs.DailyConsume;

public class AddDailyProductRequest
{
    // The request identifies the client, selected meal type, approved product, and consumed portions.
    public int ClientId { get; set; }

    public int MealTimeId { get; set; }

    public string ProductCode { get; set; } = string.Empty;

    public decimal Quantity { get; set; }
}
