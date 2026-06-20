namespace NutriTEC.Application.DTOs.Admin;

public class AdminProductMutationResponse
{
    // Status review responses confirm the action and return the refreshed admin product snapshot.
    public string Message { get; set; } = string.Empty;

    public AdminProductResponse Product { get; set; } = null!;
}
