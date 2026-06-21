namespace NutriTEC.Application.DTOs.Admin;

public class AdminProductResponse
{
    // Admin product responses include nutrition facts plus ownership and review metadata.
    public string BarCode { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public string PortionUnit { get; set; } = string.Empty;

    public decimal PortionSize { get; set; }

    public decimal Calories { get; set; }

    public decimal Fat { get; set; }

    public decimal Sodium { get; set; }

    public decimal Carbohydrates { get; set; }

    public decimal Protein { get; set; }

    public decimal Vitamins { get; set; }

    public decimal Calcium { get; set; }

    public decimal Iron { get; set; }

    public string ProductStatus { get; set; } = string.Empty;

    public int CreatedByUserId { get; set; }

    public string CreatedByName { get; set; } = string.Empty;

    public string CreatedByEmail { get; set; } = string.Empty;
}
