namespace NutriTEC.Application.DTOs.Admin;

public class AdminBillingNutritionistChargeResponse
{
    public string BillingFrequency { get; set; } = string.Empty;

    public int NutritionistCode { get; set; }

    public string NutritionistName { get; set; } = string.Empty;

    public string NutritionistEmail { get; set; } = string.Empty;

    public string PaymentMethod { get; set; } = string.Empty;

    public string? CreditCardNumber { get; set; }

    public decimal PricePerPatient { get; set; }

    public decimal DiscountRate { get; set; }

    public int PatientCount { get; set; }

    public decimal TotalAmountBeforeDiscount { get; set; }

    public decimal DiscountApplied { get; set; }

    public decimal FinalAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public IReadOnlyCollection<AdminBillingPatientChargeResponse> Patients { get; set; } =
        Array.Empty<AdminBillingPatientChargeResponse>();
}
