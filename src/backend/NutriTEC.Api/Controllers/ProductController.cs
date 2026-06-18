using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NutriTEC.Application.DTOs.Products;
using NutriTEC.Application.Interfaces.Products;

namespace NutriTEC.Api.Controllers;

[ApiController]
[Route("api/product")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IValidator<CreateProductRequest> _createProductValidator;
    private readonly IValidator<UpdateProductRequest> _updateProductValidator;
    private readonly IValidator<DeleteProductRequest> _deleteProductValidator;

    public ProductController(
        IProductService productService,
        IValidator<CreateProductRequest> createProductValidator,
        IValidator<UpdateProductRequest> updateProductValidator,
        IValidator<DeleteProductRequest> deleteProductValidator)
    {
        _productService = productService;
        _createProductValidator = createProductValidator;
        _updateProductValidator = updateProductValidator;
        _deleteProductValidator = deleteProductValidator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        // The controller validates request shape and delegates product submission rules to the service.
        await _createProductValidator.ValidateRequestAsync(request, cancellationToken);

        var response = await _productService.CreateProductAsync(request, cancellationToken);
        return Created($"/api/product/{response.Product.BarCode}", response);
    }

    [HttpGet("pending/{userId:int}")]
    public async Task<IActionResult> GetPendingProducts(
        int userId,
        CancellationToken cancellationToken)
    {
        // Pending product listing is scoped by the submitter id from the route.
        ControllerValidationExtensions.ValidatePositiveRouteValue(
            nameof(userId),
            userId,
            "El identificador del usuario debe ser mayor que 0.");

        var response = await _productService.GetPendingProductsByUserAsync(userId, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{barCode}")]
    public async Task<IActionResult> UpdateProduct(
        string barCode,
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        // The route barcode identifies the existing product and cannot be changed by the request body.
        ControllerValidationExtensions.ValidateRequiredRouteValue(
            nameof(barCode),
            barCode,
            "El codigo de barras es obligatorio.");

        await _updateProductValidator.ValidateRequestAsync(request, cancellationToken);

        var response = await _productService.UpdatePendingProductAsync(barCode, request, cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{barCode}")]
    public async Task<IActionResult> DeleteProduct(
        string barCode,
        [FromQuery] int userId,
        CancellationToken cancellationToken)
    {
        // Delete combines the route barcode and query user id into one validated request.
        var request = new DeleteProductRequest
        {
            BarCode = barCode,
            UserId = userId
        };

        await _deleteProductValidator.ValidateRequestAsync(request, cancellationToken);

        var response = await _productService.DeletePendingProductAsync(request, cancellationToken);
        return Ok(response);
    }
}
