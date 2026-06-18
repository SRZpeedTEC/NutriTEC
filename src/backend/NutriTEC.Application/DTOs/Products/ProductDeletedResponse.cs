namespace NutriTEC.Application.DTOs.Products;

public class ProductDeletedResponse
{
    // Delete responses confirm which pending product was removed.
    public string BarCode { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}
