namespace NutriTEC.Application.DTOs.Products;

public class ProductMutationResponse
{
    // Mutation responses confirm the operation and include the current product snapshot.
    public string Message { get; set; } = string.Empty;

    public ProductResponse Product { get; set; } = null!;
}
