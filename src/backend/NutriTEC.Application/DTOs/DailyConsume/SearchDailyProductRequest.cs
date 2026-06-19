namespace NutriTEC.Application.DTOs.DailyConsume;

public class SearchDailyProductRequest
{
    // Search text may match either the product name or barcode.
    public string Query { get; set; } = string.Empty;
}
