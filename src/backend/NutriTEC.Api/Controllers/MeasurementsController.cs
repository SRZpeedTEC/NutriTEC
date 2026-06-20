using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NutriTEC.Application.DTOs.Measurements;
using NutriTEC.Application.Interfaces.Measurements;

namespace NutriTEC.Api.Controllers;

[ApiController]
[Route("api/measurements")]
public class MeasurementsController : ControllerBase
{
    private readonly IMeasurementService _measurementService;
    private readonly IValidator<CreateMeasurementRequest> _createMeasurementValidator;
    private readonly IValidator<UpdateMeasurementRequest> _updateMeasurementValidator;
    private readonly IValidator<MeasurementReportRequest> _reportValidator;

    public MeasurementsController(
        IMeasurementService measurementService,
        IValidator<CreateMeasurementRequest> createMeasurementValidator,
        IValidator<UpdateMeasurementRequest> updateMeasurementValidator,
        IValidator<MeasurementReportRequest> reportValidator)
    {
        _measurementService = measurementService;
        _createMeasurementValidator = createMeasurementValidator;
        _updateMeasurementValidator = updateMeasurementValidator;
        _reportValidator = reportValidator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateMeasurement(
        CreateMeasurementRequest request,
        CancellationToken cancellationToken)
    {
        // The controller validates request shape and delegates client/date rules to the service.
        await _createMeasurementValidator.ValidateRequestAsync(request, cancellationToken);

        var response = await _measurementService.CreateMeasurementAsync(request, cancellationToken);
        return Created($"/api/measurements/{response.Measurement.ClientId}/{response.Measurement.MeasurementDate:yyyy-MM-dd}", response);
    }

    [HttpPut("{clientId:int}/{measurementDate:datetime}")]
    public async Task<IActionResult> UpdateMeasurement(
        int clientId,
        DateTime measurementDate,
        UpdateMeasurementRequest request,
        CancellationToken cancellationToken)
    {
        // Route values identify the existing measurement while the body carries only editable metrics.
        ControllerValidationExtensions.ValidatePositiveRouteValue(
            nameof(clientId),
            clientId,
            "El identificador del cliente debe ser mayor que 0.");

        await _updateMeasurementValidator.ValidateRequestAsync(request, cancellationToken);

        var response = await _measurementService.UpdateLatestMeasurementAsync(
            clientId,
            measurementDate,
            request,
            cancellationToken);

        return Ok(response);
    }

    [HttpGet("client/{clientId:int}")]
    public async Task<IActionResult> GetClientHistory(
        int clientId,
        CancellationToken cancellationToken)
    {
        // History is scoped to a single client identifier provided by the route.
        ControllerValidationExtensions.ValidatePositiveRouteValue(
            nameof(clientId),
            clientId,
            "El identificador del cliente debe ser mayor que 0.");

        var response = await _measurementService.GetClientHistoryAsync(clientId, cancellationToken);
        return Ok(response);
    }

    [HttpGet("report/{clientId:int}")]
    public async Task<IActionResult> GetReport(
        int clientId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        CancellationToken cancellationToken)
    {
        // Query values are wrapped in a DTO so report ranges use the standard FluentValidation response shape.
        var request = new MeasurementReportRequest
        {
            ClientId = clientId,
            StartDate = startDate,
            EndDate = endDate
        };
        await _reportValidator.ValidateRequestAsync(request, cancellationToken);

        var response = await _measurementService.GetReportAsync(request, cancellationToken);
        return Ok(response);
    }
}
