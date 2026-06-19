namespace NutriTEC.Application.DTOs.DailyConsume;

public class DailyProductSearchResponse
{
    // Product selection exposes nutritional facts without review or submitter metadata.
    public string BarCode { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public string PortionUnit { get; set; } = string.Empty;

    public decimal PortionSize { get; set; }

    public decimal CaloriesPerPortion { get; set; }

    public decimal ProteinPerPortion { get; set; }

    public decimal CarbohydratesPerPortion { get; set; }

    public decimal FatPerPortion { get; set; }

    public decimal SodiumPerPortion { get; set; }
}
