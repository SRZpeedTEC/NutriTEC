namespace NutriTEC.Application.DTOs.Products;

public class DeleteProductRequest
{
    // Delete requests combine the route identifier with the submitter identity for ownership checks.
    public string BarCode { get; set; } = string.Empty;

    public int UserId { get; set; }
}
