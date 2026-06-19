using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NutriTEC.Application.Exceptions;
using NutriTEC.Application.Interfaces.DailyConsume;
using NutriTEC.Domain.Entities;
using NutriTEC.Infrastructure.Persistence;
using DailyConsumeEntity = NutriTEC.Domain.Entities.DailyConsume;

namespace NutriTEC.Infrastructure.Repositories;

public class DailyConsumeRepository : IDailyConsumeRepository
{
    private readonly NutriTecDbContext _dbContext;

    public DailyConsumeRepository(NutriTecDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<MealTime?> GetMealTimeByIdAsync(int mealTimeId, CancellationToken cancellationToken)
    {
        // The selected row is used only as a meal-type template and does not need tracking.
        return _dbContext.MealTimes
            .AsNoTracking()
            .FirstOrDefaultAsync(mealTime => mealTime.MealTimeId == mealTimeId, cancellationToken);
    }

    public Task<DailyConsumeEntity?> GetDailyConsumeAsync(
        int clientId,
        DateOnly date,
        CancellationToken cancellationToken)
    {
        // Mutation workflows keep the summary tracked so new daily links can reference it.
        return _dbContext.DailyConsumes.FirstOrDefaultAsync(
            consume => consume.ClientId == clientId && consume.ConsumeDate == date,
            cancellationToken);
    }

    public Task<DailyMealTime?> GetDailyMealTimeByTypeAsync(
        int clientId,
        DateOnly date,
        string mealType,
        CancellationToken cancellationToken)
    {
        // A client reuses one daily-scoped meal time for repeated additions to the same type and date.
        return _dbContext.DailyMealTimes
            .Include(dailyMealTime => dailyMealTime.MealTime)
            .FirstOrDefaultAsync(
                dailyMealTime => dailyMealTime.ClientId == clientId
                    && dailyMealTime.ConsumeDate == date
                    && dailyMealTime.MealTime.MealType == mealType,
                cancellationToken);
    }

    public Task<MealTimeProduct?> GetConsumedProductAsync(
        int clientId,
        int mealTimeId,
        string productCode,
        CancellationToken cancellationToken)
    {
        // Joining through daily_meal_time proves the detail is reachable by the requesting client.
        return (
            from dailyMealTime in _dbContext.DailyMealTimes
            join detail in _dbContext.MealTimeProducts
                on dailyMealTime.MealTimeId equals detail.MealTimeId
            where dailyMealTime.ClientId == clientId
                && detail.MealTimeId == mealTimeId
                && detail.ProductCode == productCode
            orderby dailyMealTime.ConsumeDate descending
            select detail)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<DateOnly?> GetLatestConsumptionDateAsync(
        int clientId,
        int mealTimeId,
        string productCode,
        CancellationToken cancellationToken)
    {
        // The latest related date determines whether the existing detail is still mutable today.
        return (
            from dailyMealTime in _dbContext.DailyMealTimes
            join detail in _dbContext.MealTimeProducts
                on dailyMealTime.MealTimeId equals detail.MealTimeId
            where dailyMealTime.ClientId == clientId
                && detail.MealTimeId == mealTimeId
                && detail.ProductCode == productCode
            select (DateOnly?)dailyMealTime.ConsumeDate)
            .MaxAsync(cancellationToken);
    }

    public Task<bool> ConsumedProductExistsAsync(
        int mealTimeId,
        string productCode,
        CancellationToken cancellationToken)
    {
        // A global existence check distinguishes missing details from details owned by another client.
        return _dbContext.MealTimeProducts
            .Where(detail => detail.MealTimeId == mealTimeId && detail.ProductCode == productCode)
            .AnyAsync(detail => _dbContext.DailyMealTimes.Any(
                dailyMealTime => dailyMealTime.MealTimeId == detail.MealTimeId), cancellationToken);
    }

    public Task<decimal> GetClientDailyGoalAsync(int clientId, CancellationToken cancellationToken)
    {
        // Daily home reads the calorie goal directly without loading the client entity graph.
        return _dbContext.Clients
            .AsNoTracking()
            .Where(client => client.ClientId == clientId)
            .Select(client => client.MaxDailyCalories)
            .SingleAsync(cancellationToken);
    }

    public async Task<decimal> GetDailyTotalAsync(
        int clientId,
        DateOnly date,
        CancellationToken cancellationToken)
    {
        // A scalar projection bypasses tracked summaries and observes the value updated by the trigger.
        return await _dbContext.DailyConsumes
            .AsNoTracking()
            .Where(consume => consume.ClientId == clientId && consume.ConsumeDate == date)
            .Select(consume => (decimal?)consume.TotalCalories)
            .SingleOrDefaultAsync(cancellationToken) ?? 0;
    }

    public async Task<IReadOnlyCollection<DailyConsumeDetail>> GetDailyDetailsAsync(
        int clientId,
        DateOnly date,
        CancellationToken cancellationToken)
    {
        // The SQL view supplies a flat read model that the service groups for the daily home response.
        return await _dbContext.DailyConsumeDetails
            .AsNoTracking()
            .Where(detail => detail.ClientId == clientId && detail.ConsumeDate == date)
            .OrderBy(detail => detail.MealType)
            .ThenBy(detail => detail.ProductName)
            .ToListAsync(cancellationToken);
    }

    public async Task AddProductAsync(
        DailyConsumeEntity? newDailyConsume,
        MealTime? newMealTime,
        DailyMealTime? newDailyMealTime,
        MealTimeProduct detail,
        CancellationToken cancellationToken)
    {
        // Two ordered saves ensure the trigger can see daily_meal_time while one transaction prevents partial data.
        await using IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            if (newDailyConsume is not null)
            {
                await _dbContext.DailyConsumes.AddAsync(newDailyConsume, cancellationToken);
            }

            if (newMealTime is not null)
            {
                await _dbContext.MealTimes.AddAsync(newMealTime, cancellationToken);
            }

            if (newDailyMealTime is not null)
            {
                await _dbContext.DailyMealTimes.AddAsync(newDailyMealTime, cancellationToken);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            await _dbContext.MealTimeProducts.AddAsync(detail, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
            when (exception.InnerException is SqlException { Number: 51001 })
        {
            // A concurrent duplicate rejected by the trigger is exposed as the API's standard conflict response.
            throw new ConflictException(
                "El cliente ya tiene un horario de comida de este tipo para el dia actual.");
        }
    }

    public void DeleteMealTimeProduct(MealTimeProduct detail)
    {
        // Consumption details use the project's existing hard-delete style.
        _dbContext.MealTimeProducts.Remove(detail);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        // Update and delete mutations require one tracked-entity save after service validation.
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
