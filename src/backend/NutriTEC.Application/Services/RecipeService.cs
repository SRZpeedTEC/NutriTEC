using NutriTEC.Application.DTOs.DailyConsume;
using NutriTEC.Application.DTOs.Recipes;
using NutriTEC.Application.Exceptions;
using NutriTEC.Application.Interfaces.Clients;
using NutriTEC.Application.Interfaces.DailyConsume;
using NutriTEC.Application.Interfaces.Products;
using NutriTEC.Application.Interfaces.Recipes;
using NutriTEC.Domain.Entities;
using NutriTEC.Domain.Enums;

namespace NutriTEC.Application.Services;

public class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IProductRepository _productRepository;
    private readonly IDailyConsumeService _dailyConsumeService;

    public RecipeService(
        IRecipeRepository recipeRepository,
        IClientRepository clientRepository,
        IProductRepository productRepository,
        IDailyConsumeService dailyConsumeService)
    {
        _recipeRepository = recipeRepository;
        _clientRepository = clientRepository;
        _productRepository = productRepository;
        _dailyConsumeService = dailyConsumeService;
    }

    public async Task<RecipeMutationResponse> CreateAsync(
        CreateRecipeRequest request,
        CancellationToken cancellationToken)
    {
        // Creation validates ownership and every approved ingredient before persisting the aggregate.
        await EnsureClientExistsAsync(request.ClientId, cancellationToken);
        Normalize(request);
        var products = await GetApprovedProductsAsync(request.Products, cancellationToken);
        EnsureTotalFitsSchema(request.Products, products);

        var recipe = new Recipe
        {
            RecipeName = request.RecipeName,
            ClientId = request.ClientId,
            TotalCalories = 0,
            Products = CreateRecipeProducts(request.Products)
        };

        await _recipeRepository.AddAsync(recipe, cancellationToken);

        return new RecipeMutationResponse
        {
            Message = "Receta creada correctamente.",
            Recipe = await CreateDetailResponseAsync(recipe.RecipeId, cancellationToken)
        };
    }

    public async Task<IReadOnlyCollection<RecipeSummaryResponse>> GetByClientAsync(
        int clientId,
        CancellationToken cancellationToken)
    {
        // Recipe lists are always scoped to an existing client profile.
        await EnsureClientExistsAsync(clientId, cancellationToken);
        var recipes = await _recipeRepository.GetByClientIdAsync(clientId, cancellationToken);

        return recipes.Select(recipe => new RecipeSummaryResponse
        {
            RecipeId = recipe.RecipeId,
            RecipeName = recipe.RecipeName,
            TotalCalories = recipe.TotalCalories,
            IngredientCount = recipe.Products.Count
        }).ToList();
    }

    public async Task<RecipeDetailResponse> GetDetailAsync(
        int recipeId,
        int clientId,
        CancellationToken cancellationToken)
    {
        // Detail reads verify ownership before exposing the view-backed ingredient composition.
        await GetOwnedRecipeAsync(recipeId, clientId, cancellationToken);
        return await CreateDetailResponseAsync(recipeId, cancellationToken);
    }

    public async Task<RecipeMutationResponse> UpdateAsync(
        int recipeId,
        UpdateRecipeRequest request,
        CancellationToken cancellationToken)
    {
        // Recipe replacement changes only the reusable template and cannot reach prior daily product rows.
        var recipe = await GetOwnedRecipeAsync(recipeId, request.ClientId, cancellationToken);
        Normalize(request);
        var products = await GetApprovedProductsAsync(request.Products, cancellationToken);
        EnsureTotalFitsSchema(request.Products, products);

        recipe.RecipeName = request.RecipeName;
        var ingredients = CreateRecipeProducts(request.Products, recipeId);
        await _recipeRepository.ReplaceProductsAsync(recipe, ingredients, cancellationToken);

        return new RecipeMutationResponse
        {
            Message = "Receta actualizada correctamente.",
            Recipe = await CreateDetailResponseAsync(recipeId, cancellationToken)
        };
    }

    public async Task<RecipeDeletedResponse> DeleteAsync(
        int recipeId,
        int clientId,
        CancellationToken cancellationToken)
    {
        // Hard deletion removes only the template and its relationships, never expanded daily records.
        var recipe = await GetOwnedRecipeAsync(recipeId, clientId, cancellationToken);
        await _recipeRepository.DeleteAsync(recipe, cancellationToken);

        return new RecipeDeletedResponse
        {
            RecipeId = recipeId,
            Message = "Receta eliminada correctamente."
        };
    }

    public async Task<DailyConsumeMutationResponse> AddToDailyConsumeAsync(
        int recipeId,
        AddRecipeToDailyConsumeRequest request,
        CancellationToken cancellationToken)
    {
        // Expansion copies ingredient quantities so later recipe edits cannot alter today's consumed products.
        var recipe = await GetOwnedRecipeAsync(recipeId, request.ClientId, cancellationToken);

        if (recipe.Products.Count == 0)
        {
            throw new ConflictException("La receta no contiene productos.");
        }

        var requestedIngredients = recipe.Products.Select(ingredient => new RecipeIngredientRequest
        {
            ProductCode = ingredient.ProductCode,
            Quantity = ingredient.Quantity
        }).ToList();
        await GetApprovedProductsAsync(requestedIngredients, cancellationToken);

        var dailyProducts = requestedIngredients.Select(ingredient =>
        {
            var scaledQuantity = Math.Round(
                ingredient.Quantity * request.Multiplier,
                2,
                MidpointRounding.AwayFromZero);

            if (scaledQuantity <= 0 || scaledQuantity > 99999999.99m)
            {
                throw new ApplicationValidationException(new Dictionary<string, string[]>
                {
                    [nameof(request.Multiplier)] = new[] { "El multiplicador produce una cantidad no permitida." }
                });
            }

            return new DailyProductBatchItem
            {
                ProductCode = ingredient.ProductCode,
                Quantity = scaledQuantity
            };
        }).ToList();

        return await _dailyConsumeService.AddProductBatchAsync(
            request.ClientId,
            request.MealType,
            dailyProducts,
            cancellationToken);
    }

    private async Task<Recipe> GetOwnedRecipeAsync(
        int recipeId,
        int clientId,
        CancellationToken cancellationToken)
    {
        // A missing recipe and a foreign recipe remain distinct API outcomes.
        await EnsureClientExistsAsync(clientId, cancellationToken);
        var recipe = await _recipeRepository.GetByIdAsync(recipeId, cancellationToken);

        if (recipe is null)
        {
            throw new NotFoundException("No se encontro la receta solicitada.");
        }

        if (recipe.ClientId != clientId)
        {
            throw new UnauthorizedException("La receta no pertenece al cliente indicado.");
        }

        return recipe;
    }

    private async Task EnsureClientExistsAsync(int clientId, CancellationToken cancellationToken)
    {
        if (!await _clientRepository.ExistsByIdAsync(clientId, cancellationToken))
        {
            throw new NotFoundException("No se encontro el cliente solicitado.");
        }
    }

    private async Task<IReadOnlyDictionary<string, Product>> GetApprovedProductsAsync(
        IReadOnlyCollection<RecipeIngredientRequest> ingredients,
        CancellationToken cancellationToken)
    {
        // One batch lookup verifies existence and active approval before any recipe write begins.
        var codes = ingredients.Select(ingredient => ingredient.ProductCode).ToList();
        var products = await _productRepository.GetByBarCodesAsync(codes, cancellationToken);

        if (products.Count != codes.Count)
        {
            throw new NotFoundException("Uno o mas productos de la receta no fueron encontrados.");
        }

        if (products.Any(product => product.ProductStatus != ProductStatus.Active))
        {
            throw new ConflictException("Todos los productos de la receta deben estar aprobados.");
        }

        return products.ToDictionary(product => product.BarCode, StringComparer.OrdinalIgnoreCase);
    }

    private async Task<RecipeDetailResponse> CreateDetailResponseAsync(
        int recipeId,
        CancellationToken cancellationToken)
    {
        // The view returns one row per ingredient and repeats safe recipe summary fields.
        var details = await _recipeRepository.GetDetailsAsync(recipeId, cancellationToken);
        var first = details.FirstOrDefault()
            ?? throw new NotFoundException("No se encontraron productos para la receta.");

        return new RecipeDetailResponse
        {
            RecipeId = first.RecipeId,
            ClientId = first.ClientId,
            RecipeName = first.RecipeName,
            TotalCalories = first.TotalCalories,
            NutritionTotals = new RecipeNutritionTotalsResponse
            {
                Calories = details.Sum(detail => detail.CalculatedCalories),
                Fat = details.Sum(detail => detail.CalculatedFat),
                Sodium = details.Sum(detail => detail.CalculatedSodium),
                Carbohydrates = details.Sum(detail => detail.CalculatedCarbohydrates),
                Protein = details.Sum(detail => detail.CalculatedProtein),
                Vitamins = CombineVitaminLists(details.Select(detail => detail.CalculatedVitamins)),
                Calcium = details.Sum(detail => detail.CalculatedCalcium),
                Iron = details.Sum(detail => detail.CalculatedIron)
            },
            Products = details.Select(detail => new RecipeIngredientResponse
            {
                BarCode = detail.BarCode,
                ProductName = detail.ProductName,
                PortionUnit = detail.PortionUnit,
                PortionSize = detail.PortionSize,
                Quantity = detail.Quantity,
                Calories = detail.CalculatedCalories,
                Fat = detail.CalculatedFat,
                Sodium = detail.CalculatedSodium,
                Carbohydrates = detail.CalculatedCarbohydrates,
                Protein = detail.CalculatedProtein,
                Vitamins = detail.CalculatedVitamins,
                Calcium = detail.CalculatedCalcium,
                Iron = detail.CalculatedIron
            }).ToList()
        };
    }

    private static List<RecipeProduct> CreateRecipeProducts(
        IEnumerable<RecipeIngredientRequest> ingredients,
        int recipeId = 0)
    {
        // Ingredient entities copy quantities rather than retaining references to request objects.
        return ingredients.Select(ingredient => new RecipeProduct
        {
            RecipeId = recipeId,
            ProductCode = ingredient.ProductCode,
            Quantity = ingredient.Quantity
        }).ToList();
    }

    private static string CombineVitaminLists(IEnumerable<string> vitaminLists)
    {
        return string.Join(
            ",",
            vitaminLists
                .SelectMany(vitamins => vitamins.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(vitamin => vitamin.Trim())
                .Where(vitamin => vitamin.Length > 0)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(vitamin => vitamin));
    }

    private static void EnsureTotalFitsSchema(
        IReadOnlyCollection<RecipeIngredientRequest> ingredients,
        IReadOnlyDictionary<string, Product> products)
    {
        // Pre-validation turns numeric overflow into a field error instead of an infrastructure exception.
        var total = ingredients.Sum(ingredient => products[ingredient.ProductCode].Calories * ingredient.Quantity);

        if (total > 99999999.99m)
        {
            throw new ApplicationValidationException(new Dictionary<string, string[]>
            {
                [nameof(CreateRecipeRequest.Products)] = new[] { "El total de calorias de la receta supera el valor permitido." }
            });
        }
    }

    private static void Normalize(CreateRecipeRequest request)
    {
        request.RecipeName = request.RecipeName.Trim();
        request.Products.ForEach(product => product.ProductCode = product.ProductCode.Trim());
    }

    private static void Normalize(UpdateRecipeRequest request)
    {
        request.RecipeName = request.RecipeName.Trim();
        request.Products.ForEach(product => product.ProductCode = product.ProductCode.Trim());
    }
}
