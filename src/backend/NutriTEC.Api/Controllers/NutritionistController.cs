using Microsoft.AspNetCore.Mvc;
using NutriTEC.Application.Exceptions;
using NutriTEC.Application.Interfaces.Nutritionists;

namespace NutriTEC.Api.Controllers;

[ApiController]
[Route("api/nutritionist")]
public class NutritionistController : ControllerBase
{
    private readonly INutritionistService _nutritionistService;
    private readonly IWebHostEnvironment _env;

    public NutritionistController(INutritionistService nutritionistService, IWebHostEnvironment env)
    {
        _nutritionistService = nutritionistService;
        _env = env;
    }

    [HttpPost("photo")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadPhoto(
        IFormFile photo,
        CancellationToken cancellationToken)
    {
        const long maxBytes = 2 * 1024 * 1024;

        if (photo is null || photo.Length == 0)
            return BadRequest(new { message = "No se recibió ningún archivo." });
        if (photo.Length > maxBytes)
            return BadRequest(new { message = "La foto no puede superar 2 MB." });

        var ct = photo.ContentType.ToLowerInvariant();
        if (ct != "image/jpeg" && ct != "image/png")
            return BadRequest(new { message = "Solo se permiten imágenes JPG o PNG." });

        var ext = ct == "image/png" ? ".png" : ".jpg";
        var fileName = $"{DateTimeOffset.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}{ext}";

        var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var folder = Path.Combine(webRoot, "uploads", "nutritionists");
        Directory.CreateDirectory(folder);

        var filePath = Path.Combine(folder, fileName);
        await using var stream = System.IO.File.Create(filePath);
        await photo.CopyToAsync(stream, cancellationToken);

        return Ok(new { url = $"/uploads/nutritionists/{fileName}" });
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
