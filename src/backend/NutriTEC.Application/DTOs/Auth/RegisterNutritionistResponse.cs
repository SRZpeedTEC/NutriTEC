namespace NutriTEC.Application.DTOs.Auth;

public class RegisterNutritionistResponse
{
    public int UserId { get; set; }

    public int NutritionistCode { get; set; }

    public int Age { get; set; }

    public string Email { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}
