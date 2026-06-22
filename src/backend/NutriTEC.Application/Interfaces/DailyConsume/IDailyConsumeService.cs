using NutriTEC.Application.DTOs.DailyConsume;

namespace NutriTEC.Application.Interfaces.DailyConsume;

public interface IDailyConsumeService
{
    // The service owns approved-product selection and client/date mutation rules.
    Task<IReadOnlyCollection<DailyProductSearchResponse>> SearchProductsAsync(
        SearchDailyProductRequest request,
        CancellationToken cancellationToken);

    Task<DailyConsumeMutationResponse> AddProductAsync(
        AddDailyProductRequest request,
        CancellationToken cancellationToken);

    Task<DailyConsumeMutationResponse> AddProductBatchAsync(
        int clientId,
        string mealType,
        IReadOnlyCollection<DailyProductBatchItem> products,
        CancellationToken cancellationToken);

    Task<DailyConsumeMutationResponse> UpdateProductAsync(
        int mealTimeId,
        string productCode,
        UpdateDailyProductRequest request,
        CancellationToken cancellationToken);

    Task<DailyConsumeMutationResponse> DeleteProductAsync(
        DeleteDailyProductRequest request,
        CancellationToken cancellationToken);

    Task<DailyConsumeTodayResponse> GetTodayAsync(int clientId, CancellationToken cancellationToken);
}
