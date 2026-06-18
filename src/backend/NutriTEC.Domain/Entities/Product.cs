using NutriTEC.Domain.Enums;

namespace NutriTEC.Domain.Entities;

public class Product
{
    // Product stores food nutrition facts submitted by users for later approval.
    public string BarCode { get; set; } = string.Empty;

    public string PortionUnit { get; set; } = string.Empty;

    public decimal Sodium { get; set; }

    public ProductStatus ProductStatus { get; set; }

    public decimal Iron { get; set; }

    public decimal Calcium { get; set; }

    public decimal Vitamins { get; set; }

    public decimal PortionSize { get; set; }

    public decimal Calories { get; set; }

    public decimal Protein { get; set; }

    public decimal Carbohydrates { get; set; }

    public decimal Fat { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public int UserId { get; set; }

    public User User { get; set; } = null!;
}
