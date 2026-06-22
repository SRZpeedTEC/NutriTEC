using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NutriTEC.Application.DTOs.Admin;
using NutriTEC.Application.DTOs.Auth;
using NutriTEC.Application.Interfaces.Admin;

namespace NutriTEC.Api.Controllers;

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly IValidator<LoginRequest> _loginValidator;
    private readonly IValidator<AdminBillingReportRequest> _billingReportValidator;

    public AdminController(
        IAdminService adminService,
        IValidator<LoginRequest> loginValidator,
        IValidator<AdminBillingReportRequest> billingReportValidator)
    {
        _adminService = adminService;
        _loginValidator = loginValidator;
        _billingReportValidator = billingReportValidator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        // Admin login shares request validation with the existing auth login endpoint.
        await _loginValidator.ValidateRequestAsync(request, cancellationToken);

        var response = await _adminService.LoginAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("products")]
    public async Task<IActionResult> GetProducts(
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        var response = await _adminService.GetProductsAsync(status, cancellationToken);
        return Ok(response);
    }

    [HttpGet("billing-report")]
    public async Task<IActionResult> GetBillingReport(
        [FromQuery] string? frequency,
        [FromQuery] DateTime cycleStartDate,
        [FromQuery] DateTime cycleEndDate,
        [FromQuery] decimal? pricePerPatient,
        CancellationToken cancellationToken)
    {
        var request = new AdminBillingReportRequest
        {
            Frequency = frequency,
            CycleStartDate = cycleStartDate,
            CycleEndDate = cycleEndDate,
            PricePerPatient = pricePerPatient
        };

        await _billingReportValidator.ValidateRequestAsync(request, cancellationToken);

        var response = await _adminService.GetBillingReportAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("products/{barCode}")]
    public async Task<IActionResult> GetProductByBarCode(
        string barCode,
        CancellationToken cancellationToken)
    {
        ControllerValidationExtensions.ValidateRequiredRouteValue(
            nameof(barCode),
            barCode,
            "El codigo de barras es obligatorio.");

        var response = await _adminService.GetProductByBarCodeAsync(barCode, cancellationToken);
        return Ok(response);
    }

    [HttpPut("products/{barCode}/approve")]
    public async Task<IActionResult> ApproveProduct(
        string barCode,
        [FromQuery] int adminId,
        CancellationToken cancellationToken)
    {
        ValidateReviewRequest(barCode, adminId);

        var response = await _adminService.ApproveProductAsync(barCode, adminId, cancellationToken);
        return Ok(response);
    }

    [HttpPut("products/{barCode}/reject")]
    public async Task<IActionResult> RejectProduct(
        string barCode,
        [FromQuery] int adminId,
        CancellationToken cancellationToken)
    {
        ValidateReviewRequest(barCode, adminId);

        var response = await _adminService.RejectProductAsync(barCode, adminId, cancellationToken);
        return Ok(response);
    }

    private static void ValidateReviewRequest(string barCode, int adminId)
    {
        ControllerValidationExtensions.ValidateRequiredRouteValue(
            nameof(barCode),
            barCode,
            "El codigo de barras es obligatorio.");

        ControllerValidationExtensions.ValidatePositiveRouteValue(
            nameof(adminId),
            adminId,
            "El identificador del administrador debe ser mayor que 0.");
    }
}
