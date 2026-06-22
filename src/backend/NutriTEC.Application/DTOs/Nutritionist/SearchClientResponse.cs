namespace NutriTEC.Application.DTOs.Nutritionist;

public class SearchClientResponse
{
    public int ClientId { get; set; }

    public int UserId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public int Age { get; set; }

    public string Country { get; set; } = string.Empty;
}
