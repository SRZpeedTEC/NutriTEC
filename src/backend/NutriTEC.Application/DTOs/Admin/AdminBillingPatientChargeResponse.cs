namespace NutriTEC.Application.DTOs.Admin;

public class AdminBillingPatientChargeResponse
{
    public int ClientId { get; set; }

    public string ClientName { get; set; } = string.Empty;

    public DateTime ActiveFrom { get; set; }

    public DateTime ActiveTo { get; set; }

    public int ActiveDays { get; set; }

    public decimal ProrationFactor { get; set; }

    public decimal AmountBeforeDiscount { get; set; }

    public decimal Amount { get; set; }
}
