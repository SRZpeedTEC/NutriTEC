using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NutriTEC.Application.DTOs.NutritionPlans;
using NutriTEC.Application.Interfaces.NutritionPlans;

namespace NutriTEC.Api.Controllers;

[ApiController]
[Route("api/nutrition-plans")]
public class NutritionPlanController : ControllerBase
{
    private readonly INutritionPlanService _planService;
    private readonly IValidator<CreateNutritionPlanRequest> _createValidator;
    private readonly IValidator<UpdateNutritionPlanRequest> _updateValidator;
    private readonly IValidator<AssignPlanRequest> _assignValidator;

    public NutritionPlanController(
        INutritionPlanService planService,
        IValidator<CreateNutritionPlanRequest> createValidator,
        IValidator<UpdateNutritionPlanRequest> updateValidator,
        IValidator<AssignPlanRequest> assignValidator)
    {
        _planService = planService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _assignValidator = assignValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateNutritionPlanRequest request,
        CancellationToken cancellationToken)
    {
        await _createValidator.ValidateRequestAsync(request, cancellationToken);

        var response = await _planService.CreateAsync(request, cancellationToken);
        return Created($"/api/nutrition-plans/{response.Plan.PlanId}", response);
    }

    [HttpGet("nutritionist/{nutritionistCode:int}")]
    public async Task<IActionResult> GetByNutritionist(
        int nutritionistCode,
        CancellationToken cancellationToken)
    {
        ControllerValidationExtensions.ValidatePositiveRouteValue(
            nameof(nutritionistCode),
            nutritionistCode,
            "El codigo del nutricionista debe ser mayor que 0.");

        var response = await _planService.GetByNutritionistAsync(nutritionistCode, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{planId:int}")]
    public async Task<IActionResult> GetDetail(
        int planId,
        CancellationToken cancellationToken)
    {
        ControllerValidationExtensions.ValidatePositiveRouteValue(
            nameof(planId),
            planId,
            "El identificador del plan debe ser mayor que 0.");

        var response = await _planService.GetDetailAsync(planId, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{planId:int}")]
    public async Task<IActionResult> Update(
        int planId,
        UpdateNutritionPlanRequest request,
        CancellationToken cancellationToken)
    {
        ControllerValidationExtensions.ValidatePositiveRouteValue(
            nameof(planId),
            planId,
            "El identificador del plan debe ser mayor que 0.");
        await _updateValidator.ValidateRequestAsync(request, cancellationToken);

        var response = await _planService.UpdateAsync(planId, request, cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{planId:int}")]
    public async Task<IActionResult> Delete(
        int planId,
        [FromQuery] int nutritionistCode,
        CancellationToken cancellationToken)
    {
        ControllerValidationExtensions.ValidatePositiveRouteValue(
            nameof(planId),
            planId,
            "El identificador del plan debe ser mayor que 0.");
        ControllerValidationExtensions.ValidatePositiveRouteValue(
            nameof(nutritionistCode),
            nutritionistCode,
            "El codigo del nutricionista debe ser mayor que 0.");

        var response = await _planService.DeleteAsync(planId, nutritionistCode, cancellationToken);
        return Ok(response);
    }

    [HttpGet("client/{clientId:int}/active")]
    public async Task<IActionResult> GetActiveByClient(
        int clientId,
        CancellationToken cancellationToken)
    {
        ControllerValidationExtensions.ValidatePositiveRouteValue(
            nameof(clientId),
            clientId,
            "El identificador del cliente debe ser mayor que 0.");

        var response = await _planService.GetActiveByClientAsync(clientId, cancellationToken);
        if (response is null) return NoContent();
        return Ok(response);
    }

    [HttpPost("{planId:int}/assign")]
    public async Task<IActionResult> AssignToClient(
        int planId,
        AssignPlanRequest request,
        CancellationToken cancellationToken)
    {
        ControllerValidationExtensions.ValidatePositiveRouteValue(
            nameof(planId),
            planId,
            "El identificador del plan debe ser mayor que 0.");
        await _assignValidator.ValidateRequestAsync(request, cancellationToken);

        var response = await _planService.AssignToClientAsync(planId, request, cancellationToken);
        return Ok(response);
    }

    [HttpDelete("assignments/{assignmentId:int}")]
    public async Task<IActionResult> CancelAssignment(
        int assignmentId,
        [FromQuery] int nutritionistCode,
        CancellationToken cancellationToken)
    {
        ControllerValidationExtensions.ValidatePositiveRouteValue(
            nameof(assignmentId),
            assignmentId,
            "El identificador de la asignacion debe ser mayor que 0.");
        ControllerValidationExtensions.ValidatePositiveRouteValue(
            nameof(nutritionistCode),
            nutritionistCode,
            "El codigo del nutricionista debe ser mayor que 0.");

        var response = await _planService.CancelAssignmentAsync(assignmentId, nutritionistCode, cancellationToken);
        return Ok(response);
    }
}
