namespace NutriTEC.Application.DTOs.DailyConsume;

public class DailyProductBatchItem
{
    // Batch items let recipes reuse daily-consumption rules without exposing recipe persistence details.
    public string ProductCode { get; set; } = string.Empty;

    public decimal Quantity { get; set; }
}
