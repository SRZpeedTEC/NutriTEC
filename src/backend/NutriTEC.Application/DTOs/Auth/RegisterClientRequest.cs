namespace NutriTEC.Application.DTOs.Auth;

public class RegisterClientRequest
{
    // The request combines account credentials with the first client profile snapshot.
    public string Name { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public DateOnly Birthday { get; set; }

    public string Country { get; set; } = string.Empty;

    public decimal BodyWeight { get; set; }

    public decimal BodyMassIndex { get; set; }

    public decimal Waist { get; set; }

    public decimal Neck { get; set; }

    public decimal Hip { get; set; }

    public decimal MusclePercentage { get; set; }

    public decimal FatPercentage { get; set; }

    public decimal MaxDailyCalories { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}
