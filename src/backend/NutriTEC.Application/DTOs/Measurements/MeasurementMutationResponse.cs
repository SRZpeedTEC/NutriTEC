namespace NutriTEC.Application.DTOs.Measurements;

public class MeasurementMutationResponse
{
    // Mutation responses pair a clear confirmation message with the persisted measurement snapshot.
    public string Message { get; set; } = string.Empty;

    public MeasurementResponse Measurement { get; set; } = null!;
}
