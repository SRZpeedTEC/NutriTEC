namespace NutriTEC.Application.DTOs.Measurements;

public class UpdateMeasurementRequest
{
    // Update requests contain only mutable body metrics; the route keeps the measurement identity stable.
    public decimal BodyWeight { get; set; }

    public decimal BodyMassIndex { get; set; }

    public decimal Waist { get; set; }

    public decimal Neck { get; set; }

    public decimal Hip { get; set; }

    public decimal MusclePercentage { get; set; }

    public decimal FatPercentage { get; set; }
}
