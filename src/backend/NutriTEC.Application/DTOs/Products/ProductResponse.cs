namespace NutriTEC.Application.DTOs.Products;

public class ProductResponse
{
    // Product responses expose the persisted nutrition facts and review status.
    public string BarCode { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public string PortionUnit { get; set; } = string.Empty;

    public decimal PortionSize { get; set; }

    public decimal Calories { get; set; }

    public decimal Fat { get; set; }

    public decimal Sodium { get; set; }

    public decimal Carbohydrates { get; set; }

    public decimal Protein { get; set; }

    public string Vitamins { get; set; } = string.Empty;

    public decimal Calcium { get; set; }

    public decimal Iron { get; set; }

    public string ProductStatus { get; set; } = string.Empty;

    public int UserId { get; set; }
}
