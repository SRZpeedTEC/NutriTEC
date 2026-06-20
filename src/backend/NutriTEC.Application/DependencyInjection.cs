using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using NutriTEC.Application.DTOs.Auth;
using NutriTEC.Application.DTOs.DailyConsume;
using NutriTEC.Application.DTOs.Measurements;
using NutriTEC.Application.DTOs.Products;
using NutriTEC.Application.DTOs.Recipes;
using NutriTEC.Application.Interfaces.Auth;
using NutriTEC.Application.Interfaces.DailyConsume;
using NutriTEC.Application.Interfaces.Measurements;
using NutriTEC.Application.Interfaces.Products;
using NutriTEC.Application.Interfaces.Recipes;
using NutriTEC.Application.Mappings;
using NutriTEC.Application.Services;
using NutriTEC.Application.Validators.Auth;
using NutriTEC.Application.Validators.DailyConsume;
using NutriTEC.Application.Validators.Measurements;
using NutriTEC.Application.Validators.Products;
using NutriTEC.Application.Validators.Recipes;

namespace NutriTEC.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Application services expose business workflows and the supporting validation/mapping rules.
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IMeasurementService, MeasurementService>();
        services.AddScoped<IDailyConsumeService, DailyConsumeService>();
        services.AddScoped<IRecipeService, RecipeService>();
        services.AddScoped<IValidator<RegisterClientRequest>, RegisterClientRequestValidator>();
        services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();
        services.AddScoped<IValidator<CreateProductRequest>, CreateProductRequestValidator>();
        services.AddScoped<IValidator<UpdateProductRequest>, UpdateProductRequestValidator>();
        services.AddScoped<IValidator<DeleteProductRequest>, DeleteProductRequestValidator>();
        services.AddScoped<IValidator<CreateMeasurementRequest>, CreateMeasurementRequestValidator>();
        services.AddScoped<IValidator<UpdateMeasurementRequest>, UpdateMeasurementRequestValidator>();
        services.AddScoped<IValidator<MeasurementReportRequest>, MeasurementReportRequestValidator>();
        services.AddScoped<IValidator<SearchDailyProductRequest>, SearchDailyProductRequestValidator>();
        services.AddScoped<IValidator<AddDailyProductRequest>, AddDailyProductRequestValidator>();
        services.AddScoped<IValidator<UpdateDailyProductRequest>, UpdateDailyProductRequestValidator>();
        services.AddScoped<IValidator<DeleteDailyProductRequest>, DeleteDailyProductRequestValidator>();
        services.AddScoped<IValidator<CreateRecipeRequest>, CreateRecipeRequestValidator>();
        services.AddScoped<IValidator<UpdateRecipeRequest>, UpdateRecipeRequestValidator>();
        services.AddScoped<IValidator<AddRecipeToDailyConsumeRequest>, AddRecipeToDailyConsumeRequestValidator>();
        services.AddAutoMapper(_ => { }, typeof(AuthMappingProfile).Assembly);

        return services;
    }
}
