namespace NutriTEC.Application.DTOs.Admin;

public class AdminBillingChargeRecord
{
    public string BillingFrequency { get; set; } = string.Empty;

    public int NutritionistCode { get; set; }

    public string NutritionistName { get; set; } = string.Empty;

    public string NutritionistEmail { get; set; } = string.Empty;

    public string PaymentMethod { get; set; } = string.Empty;

    public string? CreditCardNumber { get; set; }

    public decimal PricePerPatient { get; set; }

    public decimal DiscountRate { get; set; }

    public int ClientId { get; set; }

    public string ClientName { get; set; } = string.Empty;

    public DateTime ActiveFrom { get; set; }

    public DateTime ActiveTo { get; set; }

    public int ActiveDays { get; set; }

    public decimal ProrationFactor { get; set; }

    public decimal AmountBeforeDiscount { get; set; }

    public decimal Amount { get; set; }
}
