namespace NutriTEC.Application.DTOs.Auth;

public class RegisterNutritionistRequest
{
    public string Name { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public DateOnly Birthday { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string IdNumber { get; set; } = string.Empty;

    public decimal Weight { get; set; }

    public decimal BodyMassIndex { get; set; }

    public string Address { get; set; } = string.Empty;

    public string? Photo { get; set; }

    public string EncryptedCreditCard { get; set; } = string.Empty;

    public string BillingFrequency { get; set; } = string.Empty;
}
