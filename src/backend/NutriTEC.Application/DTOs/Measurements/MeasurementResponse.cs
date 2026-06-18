namespace NutriTEC.Application.DTOs.Measurements;

public class MeasurementResponse
{
    // Response DTOs expose measurement data without returning the EF domain entity directly.
    public int ClientId { get; set; }

    public DateTime MeasurementDate { get; set; }

    public decimal BodyWeight { get; set; }

    public decimal BodyMassIndex { get; set; }

    public decimal Waist { get; set; }

    public decimal Neck { get; set; }

    public decimal Hip { get; set; }

    public decimal MusclePercentage { get; set; }

    public decimal FatPercentage { get; set; }
}
