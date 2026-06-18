namespace NutriTEC.Application.DTOs.Measurements;

public class CreateMeasurementRequest
{
    // New measurement records identify the client and the calendar date for the body snapshot.
    public int ClientId { get; set; }

    public DateTime MeasurementDate { get; set; }

    // Body metrics mirror the existing measure table columns so records can be persisted completely.
    public decimal BodyWeight { get; set; }

    public decimal BodyMassIndex { get; set; }

    public decimal Waist { get; set; }

    public decimal Neck { get; set; }

    public decimal Hip { get; set; }

    public decimal MusclePercentage { get; set; }

    public decimal FatPercentage { get; set; }
}
