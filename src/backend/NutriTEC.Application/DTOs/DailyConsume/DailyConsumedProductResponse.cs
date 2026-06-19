namespace NutriTEC.Application.DTOs.DailyConsume;

public class DailyConsumedProductResponse
{
    // Consumed-product nutrition values are scaled by quantity for the selected daily detail.
    public string BarCode { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public string PortionUnit { get; set; } = string.Empty;

    public decimal PortionSize { get; set; }

    public decimal Quantity { get; set; }

    public decimal Calories { get; set; }

    public decimal Protein { get; set; }

    public decimal Carbohydrates { get; set; }

    public decimal Fat { get; set; }

    public decimal Sodium { get; set; }
}
