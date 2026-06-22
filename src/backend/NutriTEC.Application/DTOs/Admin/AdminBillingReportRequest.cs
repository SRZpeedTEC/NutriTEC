namespace NutriTEC.Application.DTOs.Admin;

public class AdminBillingReportRequest
{
    public string? Frequency { get; set; }

    public DateTime CycleStartDate { get; set; }

    public DateTime CycleEndDate { get; set; }

    public decimal? PricePerPatient { get; set; }
}
