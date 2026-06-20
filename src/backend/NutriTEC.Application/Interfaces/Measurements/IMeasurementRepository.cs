using NutriTEC.Domain.Entities;

namespace NutriTEC.Application.Interfaces.Measurements;

public interface IMeasurementRepository
{
    // Date-based checks use calendar-day ranges because the feature exposes measurement dates to the frontend.
    Task<bool> MeasurementDateExistsAsync(
        int clientId,
        DateTime measurementDate,
        CancellationToken cancellationToken);

    Task<Measure?> GetByClientIdAndDateAsync(
        int clientId,
        DateTime measurementDate,
        CancellationToken cancellationToken);

    Task<Measure?> GetFirstByClientIdAsync(
        int clientId,
        CancellationToken cancellationToken);

    Task<Measure?> GetLatestByClientIdAsync(
        int clientId,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Measure>> GetByClientIdAsync(
        int clientId,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Measure>> GetByClientIdAndRangeAsync(
        int clientId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken);

    Task AddAsync(Measure measure, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
