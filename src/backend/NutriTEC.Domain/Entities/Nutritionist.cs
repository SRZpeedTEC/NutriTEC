namespace NutriTEC.Domain.Entities;

public class Nutritionist
{
    public int NutritionistCode { get; set; }

    public string PaymentMethod { get; set; } = string.Empty;

    public string BillingFrequency { get; set; } = "MONTHLY";

    public string? Photo { get; set; }

    public string Address { get; set; } = string.Empty;

    public string IdNumber { get; set; } = string.Empty;

    public string? EncryptedCreditCard { get; set; }

    public decimal Weight { get; set; }

    public decimal BodyMassIndex { get; set; }

    public int UserId { get; set; }

    public User User { get; set; } = null!;

    public ICollection<NutritionistClient> Patients { get; set; } = new List<NutritionistClient>();

    public ICollection<NutritionPlan> NutritionPlans { get; set; } = new List<NutritionPlan>();
}
