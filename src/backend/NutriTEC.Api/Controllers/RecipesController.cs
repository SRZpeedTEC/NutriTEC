using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NutriTEC.Application.DTOs.Recipes;
using NutriTEC.Application.Interfaces.Recipes;

namespace NutriTEC.Api.Controllers;

[ApiController]
[Route("api/recipes")]
public class RecipesController : ControllerBase
{
    private readonly IRecipeService _recipeService;
    private readonly IValidator<CreateRecipeRequest> _createValidator;
    private readonly IValidator<UpdateRecipeRequest> _updateValidator;
    private readonly IValidator<AddRecipeToDailyConsumeRequest> _dailyConsumeValidator;

    public RecipesController(
        IRecipeService recipeService,
        IValidator<CreateRecipeRequest> createValidator,
        IValidator<UpdateRecipeRequest> updateValidator,
        IValidator<AddRecipeToDailyConsumeRequest> dailyConsumeValidator)
    {
        _recipeService = recipeService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _dailyConsumeValidator = dailyConsumeValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateRecipeRequest request,
        CancellationToken cancellationToken)
    {
        // The controller validates request shape while the service owns product approval and persistence rules.
        await _createValidator.ValidateRequestAsync(request, cancellationToken);
        var response = await _recipeService.CreateAsync(request, cancellationToken);
        return Created($"/api/recipes/{response.Recipe.RecipeId}?clientId={request.ClientId}", response);
    }

    [HttpGet("client/{clientId:int}")]
    public async Task<IActionResult> GetByClient(
        int clientId,
        CancellationToken cancellationToken)
    {
        // Client recipe lists are scoped by the positive route identifier.
        ControllerValidationExtensions.ValidatePositiveRouteValue(
            nameof(clientId),
            clientId,
            "El identificador del cliente debe ser mayor que 0.");

        var response = await _recipeService.GetByClientAsync(clientId, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{recipeId:int}")]
    public async Task<IActionResult> GetDetail(
        int recipeId,
        [FromQuery] int clientId,
        CancellationToken cancellationToken)
    {
        // Both identifiers are required because recipe detail is private to its owning client.
        ValidateRecipeAndClientIds(recipeId, clientId);
        var response = await _recipeService.GetDetailAsync(recipeId, clientId, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{recipeId:int}")]
    public async Task<IActionResult> Update(
        int recipeId,
        UpdateRecipeRequest request,
        CancellationToken cancellationToken)
    {
        // The route fixes recipe identity while the body replaces editable template content.
        ControllerValidationExtensions.ValidatePositiveRouteValue(
            nameof(recipeId),
            recipeId,
            "El identificador de la receta debe ser mayor que 0.");
        await _updateValidator.ValidateRequestAsync(request, cancellationToken);

        var response = await _recipeService.UpdateAsync(recipeId, request, cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{recipeId:int}")]
    public async Task<IActionResult> Delete(
        int recipeId,
        [FromQuery] int clientId,
        CancellationToken cancellationToken)
    {
        // Hard deletion uses route and query identifiers to enforce ownership before persistence.
        ValidateRecipeAndClientIds(recipeId, clientId);
        var response = await _recipeService.DeleteAsync(recipeId, clientId, cancellationToken);
        return Ok(response);
    }

    [HttpPost("{recipeId:int}/daily-consume")]
    public async Task<IActionResult> AddToDailyConsume(
        int recipeId,
        AddRecipeToDailyConsumeRequest request,
        CancellationToken cancellationToken)
    {
        // Recipe expansion delegates its batch insertion to the existing daily-consumption workflow.
        ControllerValidationExtensions.ValidatePositiveRouteValue(
            nameof(recipeId),
            recipeId,
            "El identificador de la receta debe ser mayor que 0.");
        await _dailyConsumeValidator.ValidateRequestAsync(request, cancellationToken);

        var response = await _recipeService.AddToDailyConsumeAsync(recipeId, request, cancellationToken);
        return Ok(response);
    }

    private static void ValidateRecipeAndClientIds(int recipeId, int clientId)
    {
        // Shared route validation keeps detail and deletion error shapes consistent.
        ControllerValidationExtensions.ValidatePositiveRouteValue(
            nameof(recipeId),
            recipeId,
            "El identificador de la receta debe ser mayor que 0.");
        ControllerValidationExtensions.ValidatePositiveRouteValue(
            nameof(clientId),
            clientId,
            "El identificador del cliente debe ser mayor que 0.");
    }
}
