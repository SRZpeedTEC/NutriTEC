namespace NutriTEC.Domain.Entities;

public class DailyConsumeDetail
{
    // This read-only model mirrors vw_daily_consume_detail for daily home projections.
    public int ClientId { get; set; }

    public DateOnly ConsumeDate { get; set; }

    public int MealTimeId { get; set; }

    public string MealType { get; set; } = string.Empty;

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
