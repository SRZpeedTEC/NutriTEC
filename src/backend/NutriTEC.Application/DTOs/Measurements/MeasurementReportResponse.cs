namespace NutriTEC.Application.DTOs.Measurements;

public class MeasurementReportResponse
{
    // The frontend receives normalized range metadata and every snapshot needed to render or export its report.
    public int ClientId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int RecordCount { get; set; }

    public IReadOnlyCollection<MeasurementResponse> Measurements { get; set; } = Array.Empty<MeasurementResponse>();
}
