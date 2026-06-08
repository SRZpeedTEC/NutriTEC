namespace NutriTEC.Application.DTOs.Products;

public class CreateProductRequest
{
    // Product creation receives the nutrition facts and the submitting user identifier.
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

    public int UserId { get; set; }
}
