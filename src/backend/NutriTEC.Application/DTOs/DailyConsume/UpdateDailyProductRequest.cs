namespace NutriTEC.Application.DTOs.DailyConsume;

public class UpdateDailyProductRequest
{
    // Ownership remains explicit because authentication claims are not yet available in this API.
    public int ClientId { get; set; }

    public decimal Quantity { get; set; }
}
