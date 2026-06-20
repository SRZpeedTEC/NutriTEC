namespace NutriTEC.Application.DTOs.Measurements;

public class MeasurementReportRequest
{
    // The report request identifies one client and an inclusive calendar-date interval.
    public int ClientId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }
}
