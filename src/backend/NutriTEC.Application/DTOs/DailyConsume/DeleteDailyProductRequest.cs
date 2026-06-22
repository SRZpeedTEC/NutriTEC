namespace NutriTEC.Application.DTOs.DailyConsume;

public class DeleteDailyProductRequest
{
    // The existing composite detail key is combined with the requesting client for ownership checks.
    public int ClientId { get; set; }

    public int MealTimeId { get; set; }

    public string ProductCode { get; set; } = string.Empty;
}
