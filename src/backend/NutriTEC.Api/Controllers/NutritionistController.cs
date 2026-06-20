using Microsoft.AspNetCore.Mvc;
using NutriTEC.Application.Exceptions;
using NutriTEC.Application.Interfaces.Nutritionists;

namespace NutriTEC.Api.Controllers;

[ApiController]
[Route("api/nutritionist")]
public class NutritionistController : ControllerBase
{
    private readonly INutritionistService _nutritionistService;

    public NutritionistController(INutritionistService nutritionistService)
    {
        _nutritionistService = nutritionistService;
    }

    [HttpGet("clients/search")]
    public async Task<IActionResult> SearchClients(
        [FromQuery] string query,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Trim().Length < 2)
        {
            throw new ApplicationValidationException(new Dictionary<string, string[]>
            {
                [nameof(query)] = ["La busqueda debe tener al menos 2 caracteres."]
            });
        }

        var response = await _nutritionistService.SearchClientsAsync(query, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{nutritionistCode:int}/patients")]
    public async Task<IActionResult> GetPatients(
        int nutritionistCode,
        CancellationToken cancellationToken)
    {
        ControllerValidationExtensions.ValidatePositiveRouteValue(
            nameof(nutritionistCode),
            nutritionistCode,
            "El codigo del nutricionista debe ser mayor que 0.");

        var response = await _nutritionistService.GetPatientsAsync(nutritionistCode, cancellationToken);
        return Ok(response);
    }

    [HttpPost("{nutritionistCode:int}/patients/{clientId:int}")]
    public async Task<IActionResult> AssociatePatient(
        int nutritionistCode,
        int clientId,
        CancellationToken cancellationToken)
    {
        ControllerValidationExtensions.ValidatePositiveRouteValue(
            nameof(nutritionistCode),
            nutritionistCode,
            "El codigo del nutricionista debe ser mayor que 0.");
        ControllerValidationExtensions.ValidatePositiveRouteValue(
            nameof(clientId),
            clientId,
            "El identificador del cliente debe ser mayor que 0.");

        var response = await _nutritionistService.AssociatePatientAsync(nutritionistCode, clientId, cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{nutritionistCode:int}/patients/{clientId:int}")]
    public async Task<IActionResult> DisassociatePatient(
        int nutritionistCode,
        int clientId,
        CancellationToken cancellationToken)
    {
        ControllerValidationExtensions.ValidatePositiveRouteValue(
            nameof(nutritionistCode),
            nutritionistCode,
            "El codigo del nutricionista debe ser mayor que 0.");
        ControllerValidationExtensions.ValidatePositiveRouteValue(
            nameof(clientId),
            clientId,
            "El identificador del cliente debe ser mayor que 0.");

        var response = await _nutritionistService.DisassociatePatientAsync(nutritionistCode, clientId, cancellationToken);
        return Ok(response);
    }
}
