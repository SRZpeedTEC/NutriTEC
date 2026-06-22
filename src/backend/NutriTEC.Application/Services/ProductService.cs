using AutoMapper;
using NutriTEC.Application.DTOs.Products;
using NutriTEC.Application.Exceptions;
using NutriTEC.Application.Interfaces.Products;
using NutriTEC.Application.Interfaces.Users;
using NutriTEC.Domain.Entities;
using NutriTEC.Domain.Enums;

namespace NutriTEC.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public ProductService(
        IProductRepository productRepository,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<ProductMutationResponse> CreateProductAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        // Product submission normalizes text and enforces unique barcodes before persistence.
        NormalizeCreateRequest(request);

        if (await _productRepository.BarCodeExistsAsync(request.BarCode, cancellationToken))
        {
            throw new ConflictException("El codigo de barras ya esta registrado.");
        }

        if (!await _userRepository.ExistsByIdAsync(request.UserId, cancellationToken))
        {
            throw new NotFoundException("No se encontro el usuario que envia el producto.");
        }

        var product = _mapper.Map<Product>(request);
        product.ProductStatus = ProductStatus.PendingReview;

        await _productRepository.AddAsync(product, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);

        return CreateMutationResponse("Producto enviado para revision.", product);
    }

    public async Task<IReadOnlyCollection<ProductResponse>> GetPendingProductsByUserAsync(
        int userId,
        CancellationToken cancellationToken)
    {
        // Pending lists are scoped by submitter so users never see products from another account.
        if (userId <= 0)
        {
            throw new ApplicationValidationException(new Dictionary<string, string[]>
            {
                [nameof(userId)] = new[] { "El identificador del usuario debe ser mayor que 0." }
            });
        }

        var products = await _productRepository.GetPendingByUserIdAsync(userId, cancellationToken);
        return products.Select(product => _mapper.Map<ProductResponse>(product)).ToList();
    }

    public async Task<ProductMutationResponse> UpdatePendingProductAsync(
        string barCode,
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        // Editing is allowed only for the submitter while the product is pending review.
        var product = await GetEditableProductAsync(barCode.Trim(), request.UserId, cancellationToken);
        NormalizeUpdateRequest(request);

        _mapper.Map(request, product);
        await _productRepository.SaveChangesAsync(cancellationToken);

        return CreateMutationResponse("Producto actualizado correctamente.", product);
    }

    public async Task<ProductDeletedResponse> DeletePendingProductAsync(
        DeleteProductRequest request,
        CancellationToken cancellationToken)
    {
        // Deletion follows the same ownership and pending-status rules as editing.
        request.BarCode = request.BarCode.Trim();
        var product = await GetEditableProductAsync(request.BarCode, request.UserId, cancellationToken);

        _productRepository.Delete(product);
        await _productRepository.SaveChangesAsync(cancellationToken);

        return new ProductDeletedResponse
        {
            BarCode = request.BarCode,
            Message = "Producto eliminado correctamente."
        };
    }

    private async Task<Product> GetEditableProductAsync(
        string barCode,
        int userId,
        CancellationToken cancellationToken)
    {
        // This helper centralizes the rules that protect approved or foreign products from submitter changes.
        var product = await _productRepository.GetByBarCodeAsync(barCode, cancellationToken);

        if (product is null)
        {
            throw new NotFoundException("No se encontro el producto solicitado.");
        }

        if (product.UserId != userId)
        {
            throw new UnauthorizedException("No tiene permisos para modificar este producto.");
        }

        if (product.ProductStatus != ProductStatus.PendingReview)
        {
            throw new ConflictException("Solo se pueden modificar productos pendientes de aprobacion.");
        }

        return product;
    }

    private static void NormalizeCreateRequest(CreateProductRequest request)
    {
        // Text normalization happens in the service before uniqueness checks and persistence.
        request.BarCode = request.BarCode.Trim();
        request.ProductName = request.ProductName.Trim();
        request.PortionUnit = request.PortionUnit.Trim();
        request.Vitamins = NormalizeVitaminList(request.Vitamins);
    }

    private static void NormalizeUpdateRequest(UpdateProductRequest request)
    {
        // Update normalization keeps persisted product text consistent with creation.
        request.ProductName = request.ProductName.Trim();
        request.PortionUnit = request.PortionUnit.Trim();
        request.Vitamins = NormalizeVitaminList(request.Vitamins);
    }

    private static string NormalizeVitaminList(string vitamins)
    {
        return string.Join(",", vitamins.Split(',').Select(vitamin => vitamin.Trim()));
    }

    private ProductMutationResponse CreateMutationResponse(string message, Product product)
    {
        // Mutation responses reuse the same product snapshot shape for create and update.
        return new ProductMutationResponse
        {
            Message = message,
            Product = _mapper.Map<ProductResponse>(product)
        };
    }
}
