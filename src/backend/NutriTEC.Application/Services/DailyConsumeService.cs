using NutriTEC.Application.DTOs.DailyConsume;
using NutriTEC.Application.Exceptions;
using NutriTEC.Application.Interfaces.Clients;
using NutriTEC.Application.Interfaces.DailyConsume;
using NutriTEC.Application.Interfaces.Products;
using NutriTEC.Domain.Entities;
using NutriTEC.Domain.Enums;
using DailyConsumeEntity = NutriTEC.Domain.Entities.DailyConsume;

namespace NutriTEC.Application.Services;

public class DailyConsumeService : IDailyConsumeService
{
    private readonly IDailyConsumeRepository _dailyConsumeRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IProductRepository _productRepository;

    public DailyConsumeService(
        IDailyConsumeRepository dailyConsumeRepository,
        IClientRepository clientRepository,
        IProductRepository productRepository)
    {
        _dailyConsumeRepository = dailyConsumeRepository;
        _clientRepository = clientRepository;
        _productRepository = productRepository;
    }

    public async Task<IReadOnlyCollection<DailyProductSearchResponse>> SearchProductsAsync(
        SearchDailyProductRequest request,
        CancellationToken cancellationToken)
    {
        // The repository enforces active status while the service normalizes the user-entered search text.
        request.Query = request.Query.Trim();
        var products = await _productRepository.SearchActiveAsync(request.Query, cancellationToken);

        return products.Select(product => new DailyProductSearchResponse
        {
            BarCode = product.BarCode,
            ProductName = product.ProductName,
            PortionUnit = product.PortionUnit,
            PortionSize = product.PortionSize,
            CaloriesPerPortion = product.Calories,
            ProteinPerPortion = product.Protein,
            CarbohydratesPerPortion = product.Carbohydrates,
            FatPerPortion = product.Fat,
            SodiumPerPortion = product.Sodium
        }).ToList();
    }

    public async Task<DailyConsumeMutationResponse> AddProductAsync(
        AddDailyProductRequest request,
        CancellationToken cancellationToken)
    {
        // Single-product additions share the same atomic workflow used by recipe expansion.
        return await AddProductsCoreAsync(
            request.ClientId,
            request.MealType,
            new[]
            {
                new DailyProductBatchItem
                {
                    ProductCode = request.ProductCode,
                    Quantity = request.Quantity
                }
            },
            "Producto agregado al consumo diario.",
            cancellationToken);
    }

    public Task<DailyConsumeMutationResponse> AddProductBatchAsync(
        int clientId,
        string mealType,
        IReadOnlyCollection<DailyProductBatchItem> products,
        CancellationToken cancellationToken)
    {
        // Recipe expansion enters daily consumption through one all-or-nothing batch operation.
        return AddProductsCoreAsync(
            clientId,
            mealType,
            products,
            "Receta agregada al consumo diario.",
            cancellationToken);
    }

    public async Task<DailyConsumeMutationResponse> UpdateProductAsync(
        int mealTimeId,
        string productCode,
        UpdateDailyProductRequest request,
        CancellationToken cancellationToken)
    {
        // Edit authorization is derived from the client/date/meal-time relationship before changing quantity.
        productCode = productCode.Trim();
        var today = GetToday();
        var detail = await GetEditableDetailAsync(
            request.ClientId,
            mealTimeId,
            productCode,
            today,
            cancellationToken);
        var product = await _productRepository.GetByBarCodeAsync(productCode, cancellationToken)
            ?? throw new NotFoundException("No se encontro el producto solicitado.");
        var mealTime = await _dailyConsumeRepository.GetMealTimeByIdAsync(mealTimeId, cancellationToken)
            ?? throw new NotFoundException("No se encontro el horario de comida solicitado.");

        detail.Quantity = request.Quantity;
        detail.Calories = CalculateCalories(product.Calories, request.Quantity);
        await _dailyConsumeRepository.SaveChangesAsync(cancellationToken);

        return await CreateMutationResponseAsync(
            "Producto consumido actualizado correctamente.",
            request.ClientId,
            today,
            mealTimeId,
            mealTime.MealType,
            cancellationToken);
    }

    public async Task<DailyConsumeMutationResponse> DeleteProductAsync(
        DeleteDailyProductRequest request,
        CancellationToken cancellationToken)
    {
        // Delete follows the same ownership and today-only policy as editing.
        request.ProductCode = request.ProductCode.Trim();
        var today = GetToday();
        var detail = await GetEditableDetailAsync(
            request.ClientId,
            request.MealTimeId,
            request.ProductCode,
            today,
            cancellationToken);
        var mealTime = await _dailyConsumeRepository.GetMealTimeByIdAsync(
            request.MealTimeId,
            cancellationToken) ?? throw new NotFoundException("No se encontro el horario de comida solicitado.");

        _dailyConsumeRepository.DeleteMealTimeProduct(detail);
        await _dailyConsumeRepository.SaveChangesAsync(cancellationToken);

        return await CreateMutationResponseAsync(
            "Producto eliminado del consumo diario.",
            request.ClientId,
            today,
            request.MealTimeId,
            mealTime.MealType,
            cancellationToken);
    }

    public async Task<DailyConsumeTodayResponse> GetTodayAsync(
        int clientId,
        CancellationToken cancellationToken)
    {
        // Home remains useful before the first product is added by returning a zero-valued daily snapshot.
        await EnsureClientExistsAsync(clientId, cancellationToken);
        return await CreateTodayResponseAsync(clientId, GetToday(), cancellationToken);
    }

    private async Task<DailyConsumeMutationResponse> AddProductsCoreAsync(
        int clientId,
        string mealType,
        IReadOnlyCollection<DailyProductBatchItem> requestedProducts,
        string successMessage,
        CancellationToken cancellationToken)
    {
        // Shared validation prevents partial recipe expansion and keeps direct additions behaviorally identical.
        await EnsureClientExistsAsync(clientId, cancellationToken);

        // El tipo llega validado; se normaliza a mayusculas para casar con el CHECK de la base de datos.
        mealType = mealType.Trim().ToUpperInvariant();

        if (requestedProducts.Count == 0)
        {
            throw new ConflictException("No hay productos disponibles para agregar al consumo diario.");
        }

        var normalizedProducts = requestedProducts
            .Select(item => new DailyProductBatchItem
            {
                ProductCode = item.ProductCode.Trim(),
                Quantity = item.Quantity
            })
            .ToList();
        var duplicatedCode = normalizedProducts
            .GroupBy(item => item.ProductCode, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault(group => group.Count() > 1)?.Key;

        if (duplicatedCode is not null)
        {
            throw new ConflictException("No se pueden agregar productos duplicados en una misma operacion.");
        }

        var productCodes = normalizedProducts.Select(item => item.ProductCode).ToList();
        var products = await _productRepository.GetByBarCodesAsync(productCodes, cancellationToken);

        if (products.Count != productCodes.Count)
        {
            throw new NotFoundException("Uno o mas productos no fueron encontrados.");
        }

        if (products.Any(product => product.ProductStatus != ProductStatus.Active))
        {
            throw new ConflictException("Todos los productos deben estar aprobados para consumo.");
        }

        var productByCode = products.ToDictionary(product => product.BarCode, StringComparer.OrdinalIgnoreCase);
        var today = GetToday();
        var dailyConsume = await _dailyConsumeRepository.GetDailyConsumeAsync(clientId, today, cancellationToken);
        var dailyMealTime = await _dailyConsumeRepository.GetDailyMealTimeByTypeAsync(
            clientId,
            today,
            mealType,
            cancellationToken);

        if (dailyMealTime is not null)
        {
            foreach (var item in normalizedProducts)
            {
                var existingDetail = await _dailyConsumeRepository.GetConsumedProductAsync(
                    clientId,
                    dailyMealTime.MealTimeId,
                    item.ProductCode,
                    cancellationToken);

                if (existingDetail is not null)
                {
                    throw new ConflictException(
                        $"El producto {item.ProductCode} ya fue agregado a este horario de comida hoy.");
                }
            }
        }

        DailyConsumeEntity? newDailyConsume = null;
        MealTime? newMealTime = null;
        DailyMealTime? newDailyMealTime = null;

        // A scoped meal-time instance prevents recipe ingredients from leaking into plans or other clients.
        if (dailyConsume is null)
        {
            dailyConsume = new DailyConsumeEntity
            {
                ClientId = clientId,
                ConsumeDate = today,
                TotalCalories = 0
            };
            newDailyConsume = dailyConsume;
        }

        if (dailyMealTime is null)
        {
            var scopedMealTime = new MealTime { MealType = mealType };
            dailyMealTime = new DailyMealTime
            {
                ClientId = clientId,
                ConsumeDate = today,
                DailyConsume = dailyConsume,
                MealTime = scopedMealTime
            };
            newMealTime = scopedMealTime;
            newDailyMealTime = dailyMealTime;
        }

        var details = normalizedProducts.Select(item => new MealTimeProduct
        {
            MealTime = dailyMealTime.MealTime,
            ProductCode = item.ProductCode,
            Quantity = item.Quantity,
            Calories = CalculateCalories(productByCode[item.ProductCode].Calories, item.Quantity)
        }).ToList();

        await _dailyConsumeRepository.AddProductsAsync(
            newDailyConsume,
            newMealTime,
            newDailyMealTime,
            details,
            cancellationToken);

        return await CreateMutationResponseAsync(
            successMessage,
            clientId,
            today,
            dailyMealTime.MealTimeId,
            dailyMealTime.MealTime.MealType,
            cancellationToken);
    }

    private async Task<MealTimeProduct> GetEditableDetailAsync(
        int clientId,
        int mealTimeId,
        string productCode,
        DateOnly today,
        CancellationToken cancellationToken)
    {
        // This sequence separates foreign ownership, missing details, and immutable historical consumption.
        await EnsureClientExistsAsync(clientId, cancellationToken);
        var latestDate = await _dailyConsumeRepository.GetLatestConsumptionDateAsync(
            clientId,
            mealTimeId,
            productCode,
            cancellationToken);

        if (latestDate is null)
        {
            if (await _dailyConsumeRepository.ConsumedProductExistsAsync(
                mealTimeId,
                productCode,
                cancellationToken))
            {
                throw new UnauthorizedException("El producto consumido no pertenece al cliente indicado.");
            }

            throw new NotFoundException("No se encontro el producto consumido solicitado.");
        }

        if (latestDate.Value != today)
        {
            throw new ConflictException("Solo se pueden modificar productos consumidos durante el dia actual.");
        }

        return await _dailyConsumeRepository.GetConsumedProductAsync(
            clientId,
            mealTimeId,
            productCode,
            cancellationToken) ?? throw new NotFoundException("No se encontro el producto consumido solicitado.");
    }

    private async Task EnsureClientExistsAsync(int clientId, CancellationToken cancellationToken)
    {
        // Client-scoped operations fail before any related database work when the profile is missing.
        if (!await _clientRepository.ExistsByIdAsync(clientId, cancellationToken))
        {
            throw new NotFoundException("No se encontro el cliente solicitado.");
        }
    }

    private async Task<DailyConsumeMutationResponse> CreateMutationResponseAsync(
        string message,
        int clientId,
        DateOnly date,
        int mealTimeId,
        string mealType,
        CancellationToken cancellationToken)
    {
        // Re-querying after SaveChanges observes both the view and trigger-maintained summary values.
        var details = await _dailyConsumeRepository.GetDailyDetailsAsync(clientId, date, cancellationToken);
        var totalCalories = await _dailyConsumeRepository.GetDailyTotalAsync(clientId, date, cancellationToken);

        return new DailyConsumeMutationResponse
        {
            Message = message,
            TotalDailyCalories = totalCalories,
            MealTime = CreateMealTimeResponse(mealTimeId, mealType, details)
        };
    }

    private async Task<DailyConsumeTodayResponse> CreateTodayResponseAsync(
        int clientId,
        DateOnly date,
        CancellationToken cancellationToken)
    {
        // Summary and detail reads remain separate because only the detail view contains product rows.
        var goal = await _dailyConsumeRepository.GetClientDailyGoalAsync(clientId, cancellationToken);
        var total = await _dailyConsumeRepository.GetDailyTotalAsync(clientId, date, cancellationToken);
        var details = await _dailyConsumeRepository.GetDailyDetailsAsync(clientId, date, cancellationToken);
        var mealTimes = details
            .GroupBy(detail => new { detail.MealTimeId, detail.MealType })
            .Select(group => CreateMealTimeResponse(group.Key.MealTimeId, group.Key.MealType, group))
            .OrderBy(mealTime => GetMealTypeOrder(mealTime.MealType))
            .ToList();

        return new DailyConsumeTodayResponse
        {
            ClientId = clientId,
            ConsumeDate = date,
            TotalCalories = total,
            MaxDailyCalories = goal,
            RemainingCalories = goal - total,
            MealTimes = mealTimes
        };
    }

    private static DailyMealTimeResponse CreateMealTimeResponse(
        int mealTimeId,
        string mealType,
        IEnumerable<DailyConsumeDetail> details)
    {
        // Flat view rows are converted into the nested shape expected by the daily consumption screen.
        var products = details
            .Where(detail => detail.MealTimeId == mealTimeId)
            .Select(detail => new DailyConsumedProductResponse
            {
                BarCode = detail.BarCode,
                ProductName = detail.ProductName,
                PortionUnit = detail.PortionUnit,
                PortionSize = detail.PortionSize,
                Quantity = detail.Quantity,
                Calories = detail.Calories,
                Protein = ScaleNutrition(detail.Protein, detail.Quantity),
                Carbohydrates = ScaleNutrition(detail.Carbohydrates, detail.Quantity),
                Fat = ScaleNutrition(detail.Fat, detail.Quantity),
                Sodium = ScaleNutrition(detail.Sodium, detail.Quantity)
            })
            .OrderBy(product => product.ProductName)
            .ToList();

        return new DailyMealTimeResponse
        {
            MealTimeId = mealTimeId,
            MealType = mealType,
            TotalCalories = products.Sum(product => product.Calories),
            Products = products
        };
    }

    private static decimal CalculateCalories(decimal caloriesPerPortion, decimal quantity)
    {
        // Stored detail calories represent the selected number of product portions.
        var calories = Math.Round(caloriesPerPortion * quantity, 2, MidpointRounding.AwayFromZero);

        if (calories > 99999999.99m)
        {
            throw new ApplicationValidationException(new Dictionary<string, string[]>
            {
                [nameof(AddDailyProductRequest.Quantity)] = new[]
                {
                    "La cantidad produce un total de calorias superior al valor permitido."
                }
            });
        }

        return calories;
    }

    private static decimal ScaleNutrition(decimal valuePerPortion, decimal quantity)
    {
        return Math.Round(valuePerPortion * quantity, 2, MidpointRounding.AwayFromZero);
    }

    private static DateOnly GetToday()
    {
        // The existing backend uses server-local dates for rules expressed as calendar days.
        return DateOnly.FromDateTime(DateTime.Today);
    }

    private static int GetMealTypeOrder(string mealType)
    {
        // Stable ordering keeps the daily home aligned with the natural sequence of meals.
        return mealType switch
        {
            "BREAKFAST" => 1,
            "LUNCH" => 2,
            "DINNER" => 3,
            "SNACK" => 4,
            _ => 5
        };
    }
}
