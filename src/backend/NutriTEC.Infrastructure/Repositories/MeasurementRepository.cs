using Microsoft.EntityFrameworkCore;
using NutriTEC.Application.Interfaces.Measurements;
using NutriTEC.Domain.Entities;
using NutriTEC.Infrastructure.Persistence;

namespace NutriTEC.Infrastructure.Repositories;

public class MeasurementRepository : IMeasurementRepository
{
    private readonly NutriTecDbContext _dbContext;

    public MeasurementRepository(NutriTecDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> MeasurementDateExistsAsync(
        int clientId,
        DateTime measurementDate,
        CancellationToken cancellationToken)
    {
        // Calendar-day ranges avoid duplicate snapshots when stored datetimes include different times.
        var startDate = measurementDate.Date;
        var endDate = startDate.AddDays(1);

        return _dbContext.Measures
            .AnyAsync(
                measure => measure.ClientId == clientId
                    && measure.MeasureDateTime >= startDate
                    && measure.MeasureDateTime < endDate,
                cancellationToken);
    }

    public Task<Measure?> GetByClientIdAndDateAsync(
        int clientId,
        DateTime measurementDate,
        CancellationToken cancellationToken)
    {
        // Update workflows need a tracked entity for the requested calendar day.
        var startDate = measurementDate.Date;
        var endDate = startDate.AddDays(1);

        return _dbContext.Measures
            .Where(measure => measure.ClientId == clientId)
            .Where(measure => measure.MeasureDateTime >= startDate)
            .Where(measure => measure.MeasureDateTime < endDate)
            .OrderByDescending(measure => measure.MeasureDateTime)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<Measure?> GetFirstByClientIdAsync(
        int clientId,
        CancellationToken cancellationToken)
    {
        // The earliest measurement acts as the available registration-date boundary for this schema.
        return _dbContext.Measures
            .AsNoTracking()
            .Where(measure => measure.ClientId == clientId)
            .OrderBy(measure => measure.MeasureDateTime)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<Measure?> GetLatestByClientIdAsync(
        int clientId,
        CancellationToken cancellationToken)
    {
        // Latest-record checks enforce that only the newest snapshot can be edited.
        return _dbContext.Measures
            .AsNoTracking()
            .Where(measure => measure.ClientId == clientId)
            .OrderByDescending(measure => measure.MeasureDateTime)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Measure>> GetByClientIdAsync(
        int clientId,
        CancellationToken cancellationToken)
    {
        // History queries are read-only and ordered chronologically for the frontend.
        return await _dbContext.Measures
            .AsNoTracking()
            .Where(measure => measure.ClientId == clientId)
            .OrderBy(measure => measure.MeasureDateTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Measure>> GetByClientIdAndRangeAsync(
        int clientId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken)
    {
        // The exclusive upper boundary makes the user-provided final calendar date inclusive.
        var normalizedStartDate = startDate.Date;
        var exclusiveEndDate = endDate.Date.AddDays(1);

        return await _dbContext.Measures
            .AsNoTracking()
            .Where(measure => measure.ClientId == clientId)
            .Where(measure => measure.MeasureDateTime >= normalizedStartDate)
            .Where(measure => measure.MeasureDateTime < exclusiveEndDate)
            .OrderBy(measure => measure.MeasureDateTime)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Measure measure, CancellationToken cancellationToken)
    {
        // Creation is tracked by EF until the service commits the unit of work.
        await _dbContext.Measures.AddAsync(measure, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
