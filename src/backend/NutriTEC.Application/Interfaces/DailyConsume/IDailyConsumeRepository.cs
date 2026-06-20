using NutriTEC.Domain.Entities;
using DailyConsumeEntity = NutriTEC.Domain.Entities.DailyConsume;

namespace NutriTEC.Application.Interfaces.DailyConsume;

public interface IDailyConsumeRepository
{
    // Meal-time templates provide a validated type before a daily-scoped instance is created.
    Task<MealTime?> GetMealTimeByIdAsync(int mealTimeId, CancellationToken cancellationToken);

    Task<DailyConsumeEntity?> GetDailyConsumeAsync(int clientId, DateOnly date, CancellationToken cancellationToken);

    Task<DailyMealTime?> GetDailyMealTimeByTypeAsync(
        int clientId,
        DateOnly date,
        string mealType,
        CancellationToken cancellationToken);

    Task<MealTimeProduct?> GetConsumedProductAsync(
        int clientId,
        int mealTimeId,
        string productCode,
        CancellationToken cancellationToken);

    Task<DateOnly?> GetLatestConsumptionDateAsync(
        int clientId,
        int mealTimeId,
        string productCode,
        CancellationToken cancellationToken);

    Task<bool> ConsumedProductExistsAsync(
        int mealTimeId,
        string productCode,
        CancellationToken cancellationToken);

    Task<decimal> GetClientDailyGoalAsync(int clientId, CancellationToken cancellationToken);

    Task<decimal> GetDailyTotalAsync(int clientId, DateOnly date, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<DailyConsumeDetail>> GetDailyDetailsAsync(
        int clientId,
        DateOnly date,
        CancellationToken cancellationToken);

    Task AddProductsAsync(
        DailyConsumeEntity? newDailyConsume,
        MealTime? newMealTime,
        DailyMealTime? newDailyMealTime,
        IReadOnlyCollection<MealTimeProduct> details,
        CancellationToken cancellationToken);

    void DeleteMealTimeProduct(MealTimeProduct detail);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
