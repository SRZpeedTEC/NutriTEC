using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NutriTEC.Application.DTOs.DailyConsume;
using NutriTEC.Application.Interfaces.DailyConsume;

namespace NutriTEC.Api.Controllers;

[ApiController]
[Route("api/daily-consume")]
public class DailyConsumeController : ControllerBase
{
    private readonly IDailyConsumeService _dailyConsumeService;
    private readonly IValidator<SearchDailyProductRequest> _searchValidator;
    private readonly IValidator<AddDailyProductRequest> _addValidator;
    private readonly IValidator<UpdateDailyProductRequest> _updateValidator;
    private readonly IValidator<DeleteDailyProductRequest> _deleteValidator;

    public DailyConsumeController(
        IDailyConsumeService dailyConsumeService,
        IValidator<SearchDailyProductRequest> searchValidator,
        IValidator<AddDailyProductRequest> addValidator,
        IValidator<UpdateDailyProductRequest> updateValidator,
        IValidator<DeleteDailyProductRequest> deleteValidator)
    {
        _dailyConsumeService = dailyConsumeService;
        _searchValidator = searchValidator;
        _addValidator = addValidator;
        _updateValidator = updateValidator;
        _deleteValidator = deleteValidator;
    }

    [HttpGet("products/search")]
    public async Task<IActionResult> SearchProducts(
        [FromQuery] string query,
        CancellationToken cancellationToken)
    {
        // Query input is wrapped in a DTO so search uses the same validation flow as mutations.
        var request = new SearchDailyProductRequest { Query = query };
        await _searchValidator.ValidateRequestAsync(request, cancellationToken);

        var response = await _dailyConsumeService.SearchProductsAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("products")]
    public async Task<IActionResult> AddProduct(
        AddDailyProductRequest request,
        CancellationToken cancellationToken)
    {
        // Controllers validate request shape while the service owns approval, ownership, and date rules.
        await _addValidator.ValidateRequestAsync(request, cancellationToken);

        var response = await _dailyConsumeService.AddProductAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpPut("products/{mealTimeId:int}/{productCode}")]
    public async Task<IActionResult> UpdateProduct(
        int mealTimeId,
        string productCode,
        UpdateDailyProductRequest request,
        CancellationToken cancellationToken)
    {
        // Route values retain the existing composite detail identity and cannot be changed by the body.
        ControllerValidationExtensions.ValidatePositiveRouteValue(
            nameof(mealTimeId),
            mealTimeId,
            "El identificador del horario de comida debe ser mayor que 0.");
        ControllerValidationExtensions.ValidateRequiredRouteValue(
            nameof(productCode),
            productCode,
            "El codigo del producto es obligatorio.");
        await _updateValidator.ValidateRequestAsync(request, cancellationToken);

        var response = await _dailyConsumeService.UpdateProductAsync(
            mealTimeId,
            productCode,
            request,
            cancellationToken);
        return Ok(response);
    }

    [HttpDelete("products/{mealTimeId:int}/{productCode}")]
    public async Task<IActionResult> DeleteProduct(
        int mealTimeId,
        string productCode,
        [FromQuery] int clientId,
        CancellationToken cancellationToken)
    {
        // Delete combines route and query identifiers into one validated ownership request.
        var request = new DeleteDailyProductRequest
        {
            ClientId = clientId,
            MealTimeId = mealTimeId,
            ProductCode = productCode
        };
        await _deleteValidator.ValidateRequestAsync(request, cancellationToken);

        var response = await _dailyConsumeService.DeleteProductAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("today/{clientId:int}")]
    public async Task<IActionResult> GetToday(
        int clientId,
        CancellationToken cancellationToken)
    {
        // The route identifies the client whose current daily summary should be displayed.
        ControllerValidationExtensions.ValidatePositiveRouteValue(
            nameof(clientId),
            clientId,
            "El identificador del cliente debe ser mayor que 0.");

        var response = await _dailyConsumeService.GetTodayAsync(clientId, cancellationToken);
        return Ok(response);
    }
}
