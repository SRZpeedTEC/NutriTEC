namespace NutriTEC.Domain.Entities;

public class MealTimeProduct
{
    // The composite key identifies one consumed product inside a scoped meal time.
    public int MealTimeId { get; set; }

    public string ProductCode { get; set; } = string.Empty;

    public decimal Calories { get; set; }

    public decimal Quantity { get; set; }

    public MealTime MealTime { get; set; } = null!;

    public Product Product { get; set; } = null!;
}
