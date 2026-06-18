using AutoMapper;
using NutriTEC.Application.DTOs.Products;
using NutriTEC.Domain.Entities;
using NutriTEC.Domain.Enums;

namespace NutriTEC.Application.Mappings;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        // Product mappings keep DTO/entity conversion direct while services apply workflow values.
        CreateMap<CreateProductRequest, Product>()
            .ForMember(destination => destination.ProductStatus, options => options.Ignore())
            .ForMember(destination => destination.User, options => options.Ignore());

        CreateMap<UpdateProductRequest, Product>()
            .ForMember(destination => destination.BarCode, options => options.Ignore())
            .ForMember(destination => destination.ProductStatus, options => options.Ignore())
            .ForMember(destination => destination.UserId, options => options.Ignore())
            .ForMember(destination => destination.User, options => options.Ignore());

        CreateMap<Product, ProductResponse>()
            .ForMember(destination => destination.ProductStatus, options => options.MapFrom(source => source.ProductStatus.ToDatabaseValue()));
    }
}
