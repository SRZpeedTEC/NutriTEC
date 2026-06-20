using NutriTEC.Application.DTOs.Measurements;

namespace NutriTEC.Application.Interfaces.Measurements;

public interface IMeasurementService
{
    // Measurement workflows cover client history creation, latest-record editing, and history retrieval.
    Task<MeasurementMutationResponse> CreateMeasurementAsync(
        CreateMeasurementRequest request,
        CancellationToken cancellationToken);

    Task<MeasurementMutationResponse> UpdateLatestMeasurementAsync(
        int clientId,
        DateTime measurementDate,
        UpdateMeasurementRequest request,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<MeasurementResponse>> GetClientHistoryAsync(
        int clientId,
        CancellationToken cancellationToken);

    Task<MeasurementReportResponse> GetReportAsync(
        MeasurementReportRequest request,
        CancellationToken cancellationToken);
}
