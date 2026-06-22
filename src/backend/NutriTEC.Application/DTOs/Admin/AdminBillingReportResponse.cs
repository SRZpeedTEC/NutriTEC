namespace NutriTEC.Application.DTOs.Admin;

public class AdminBillingReportResponse
{
    public string? Frequency { get; set; }

    public DateTime CycleStartDate { get; set; }

    public DateTime CycleEndDate { get; set; }

    public int CycleDays { get; set; }

    public decimal? PricePerPatient { get; set; }

    public int NutritionistCount { get; set; }

    public int PatientChargeCount { get; set; }

    public decimal TotalAmountBeforeDiscount { get; set; }

    public decimal TotalDiscountApplied { get; set; }

    public decimal FinalAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public IReadOnlyDictionary<string, IReadOnlyCollection<AdminBillingNutritionistChargeResponse>> NutritionistsByBillingFrequency { get; set; } =
        new Dictionary<string, IReadOnlyCollection<AdminBillingNutritionistChargeResponse>>();

    public IReadOnlyCollection<AdminBillingNutritionistChargeResponse> Nutritionists { get; set; } =
        Array.Empty<AdminBillingNutritionistChargeResponse>();
}
