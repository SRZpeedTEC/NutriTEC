namespace NutriTEC.Domain.Entities;

public class Measure
{
    // A registration creates the first body measurement snapshot for the new client.
    public DateTime MeasureDateTime { get; set; }

    public decimal Neck { get; set; }

    public decimal MusclePercentage { get; set; }

    public decimal BodyWeight { get; set; }

    public decimal Hip { get; set; }

    public decimal Waist { get; set; }

    public decimal FatPercentage { get; set; }

    public decimal BodyMassIndex { get; set; }

    public int ClientId { get; set; }

    public Client Client { get; set; } = null!;
}
